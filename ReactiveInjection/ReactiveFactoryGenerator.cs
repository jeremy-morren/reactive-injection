using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using ReactiveInjection.DependencyTree;
using ReactiveInjection.Tokens.Generator;

namespace ReactiveInjection;

[Generator(LanguageNames.CSharp)]
public class ReactiveFactoryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var returnKinds = context.SyntaxProvider.CreateSyntaxProvider(
            static (n, _) => n is TypeDeclarationSyntax,
            static (n, _) =>
            {
                return new
                {
                    Type = (ITypeSymbol) n.SemanticModel.GetDeclaredSymbol(n.Node)!,
                    IsPartial = ((TypeDeclarationSyntax) n.Node).Modifiers
                        .Any(t => t.IsKind(SyntaxKind.PartialKeyword))
                };
            });

        var transformed = returnKinds
            .Select(static (t, _) => new TypeSymbol(t.Type, t.IsPartial))
            .Where(t => t.GetMethods().Any(Attributes.HasReactiveFactoryAttribute));

        var collected = transformed.Collect();

        context.RegisterSourceOutput(collected, static (sourceProductionContext, factories) =>
        {
            var log = new CompilationLogProvider(sourceProductionContext);
            var builder = new FactoryDependencyTreeBuilder(log);
            var writer = new FactoryImplementationWriter(log);
            foreach (var factory in factories)
            {
                try
                {
                    if (!builder.Build(factory, out var tree))
                        continue; //Errors will already have been written to log, we can ignore
                    var name = FactoryImplementationWriter.GetName(factory);
                    var result = writer.GenerateCSharp(tree);
                    sourceProductionContext.AddSource($"{name}.g.cs", SourceText.From(result, Encoding.UTF8));
                }
                catch (Exception e)
                {
                    log.WriteError(Location.None,
                        "RI1000",
                        "Unexpected error generating view model factory",
                        "An unexpected error occurred generating the view model factory '{0}': {1}: {2}",
                        factory,
                        e.GetType(),
                        e.Message);
                }
            }
        });
    }

}