using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using ReactiveInjection.Generator;
using ReactiveInjection.Tokens;
using ReactiveInjection.Tokens.Symbols;

namespace ReactiveInjection;

[Generator(LanguageNames.CSharp)]
public class ReactiveFactoryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //We run on all changes of 'Type' or 'Constructor'
        
        var returnKinds = context.SyntaxProvider.CreateSyntaxProvider(
            static (n, _) => n is TypeDeclarationSyntax or ConstructorDeclarationSyntax,
            static (n, _) =>
            {
                if (n.Node is ConstructorDeclarationSyntax)
                    return (IType?)null;
                
                var isPartial = ((TypeDeclarationSyntax)n.Node).Modifiers
                    .Any(t => t.IsKind(SyntaxKind.PartialKeyword));
                return new TypeSymbol((ITypeSymbol)n.SemanticModel.GetDeclaredSymbol(n.Node)!, isPartial);
            });

        
        context.RegisterSourceOutput(returnKinds.Collect(), static (context, types) =>
        {
            var log = new CompilationLogProvider(context);
            var builder = new FactoryDependencyTreeBuilder(log);

            var factories = types
                .WhereNotNull()
                .Where(t => t.Attributes.Any(Attributes.IsReactiveFactoryAttribute));
            
            foreach (var factory in factories)
            {
                try
                {
                    if (!builder.Build(factory, out var tree))
                        continue; //Errors will already have been written to log, we can ignore
                    var result = FactoryImplementationWriter.GenerateCSharp(tree);
                    context.AddSource($"{factory.FullName}.g.cs", result);
                }
                catch (Exception e)
                {
                    new ErrorLogWriter(log).ErrorGeneratingFactory(factory, e);
                }
            }
        });
    }
}