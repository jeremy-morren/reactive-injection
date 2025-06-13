using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveInjection.SourceGenerators.DependencyInjection;
using ReactiveInjection.SourceGenerators.Framework;
using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.SourceGenerators.Loader;

[Generator(LanguageNames.CSharp)]
public class LoaderManagerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //We run on all changes of 'Type'
        
        var types = context.SyntaxProvider.CreateSyntaxProvider(
            static (n, _) => n is TypeDeclarationSyntax,
            static (n, _) =>
            {
                var symbol = n.SemanticModel.GetDeclaredSymbol(n.Node);
                return new TypeSymbol((ITypeSymbol)symbol!);
            });
        
        //We only care about types that have methods with LoaderRouteAttribute
        types = types.Where(t => t.Methods.Any(AttributeHelpers.HasLoaderRouteAttribute));
        
        context.RegisterImplementationSourceOutput(types.Collect(), static (context, types) =>
        {
            var log = new CompilationLogProvider(context);
            var builder = new LoaderTreeBuilder(log);
            
            try
            {
                var methods = types.SelectMany(t => t.Methods)
                    .Where(AttributeHelpers.HasLoaderRouteAttribute)
                    .ToList();
                
                if (!builder.TryBuild(methods, out var tree))
                    return; //Errors will already have been written to log, we can ignore
                var result = LoaderManagerWriter.Generate(tree);
                context.AddSource(tree.Filename, result);
            }
            catch (Exception e)
            {
                new ErrorLogWriter(log).FatalRoutingGenerationError(e);
            }
        });
    }
}