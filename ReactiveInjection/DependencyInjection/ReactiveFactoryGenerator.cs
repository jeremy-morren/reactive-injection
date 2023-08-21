using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveInjection.Framework;
using ReactiveInjection.Symbols;

namespace ReactiveInjection.DependencyInjection;

[Generator(LanguageNames.CSharp)]
internal class ReactiveFactoryGenerator : IIncrementalGenerator
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
                
                return new TypeSymbol((ITypeSymbol)n.SemanticModel.GetDeclaredSymbol(n.Node)!, 
                    ((TypeDeclarationSyntax)n.Node).IsPartial());
            });

        
        context.RegisterSourceOutput(returnKinds.Collect(), static (context, types) =>
        {
            var log = new CompilationLogProvider(context);
            var builder = new FactoryDependencyTreeBuilder(log);

            var factories = types
                .WhereNotNull()
                .Where(t => t.Attributes.Any(AttributeHelpers.IsReactiveFactoryAttribute));
            
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
                    new ErrorLogWriter(log).FatalError(factory, e);
                }
            }
        });
    }
}