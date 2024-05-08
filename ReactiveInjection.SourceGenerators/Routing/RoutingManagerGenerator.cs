using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveInjection.SourceGenerators.DependencyInjection;
using ReactiveInjection.SourceGenerators.Framework;
using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.SourceGenerators.Routing;

[Generator(LanguageNames.CSharp)]
public class RoutingManagerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //We run on all changes of 'Method'
        
        var returnKinds = context.SyntaxProvider.CreateSyntaxProvider(
            static (n, _) => n is MethodDeclarationSyntax,
            static (n, _) =>
            {
                var symbol = n.SemanticModel.GetDeclaredSymbol(n.Node);
                return new MethodSymbol((IMethodSymbol)symbol!);
            });
        
        context.RegisterSourceOutput(returnKinds.Collect(), static (context, methods) =>
        {
            var log = new CompilationLogProvider(context);
            var builder = new RoutingTreeBuilder(log);

            try
            {
                if (!builder.TryBuild(methods, out var tree))
                    return; //Errors will already have been written to log, we can ignore
                var result = RoutingManagerWriter.Generate(tree);
                context.AddSource(tree.Filename, result);
            }
            catch (Exception e)
            {
                new ErrorLogWriter(log).FatalRoutingGenerationError(e);
            }
        });
    }
}