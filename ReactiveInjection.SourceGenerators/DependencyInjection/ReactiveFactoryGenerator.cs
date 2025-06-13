using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveInjection.SourceGenerators.Framework;
using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.SourceGenerators.DependencyInjection;

[Generator(LanguageNames.CSharp)]
internal class ReactiveFactoryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //We run on all changes of 'Type'
        
        var types = context.SyntaxProvider.CreateSyntaxProvider(
            static (n, _) => n is TypeDeclarationSyntax,
            static (n, _) =>
            {
                var symbol = n.SemanticModel.GetDeclaredSymbol(n.Node);
                var syntax = (TypeDeclarationSyntax)n.Node;
                return new TypeSymbol((ITypeSymbol)symbol!, syntax.IsPartial());
            });
        
        //We only care about types with ReactiveFactoryAttribute
        types = types.Where(
            t => t != null && t.Attributes.Any(AttributeHelpers.IsReactiveFactoryAttribute));
        
        context.RegisterSourceOutput(types.Collect(), static (context, factories) =>
        {
            var log = new CompilationLogProvider(context);
            var builder = new FactoryDependencyTreeBuilder(log);

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
                    new ErrorLogWriter(log).FatalFactoryGenerationError(factory, e);
                }
            }
        });
    }
}