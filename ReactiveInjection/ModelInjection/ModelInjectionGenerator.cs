using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveInjection.Framework;
using ReactiveInjection.Symbols;

namespace ReactiveInjection.ModelInjection;

[Generator(LanguageNames.CSharp)]
internal class ModelInjectionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //We run on all changes of 'Property'

        var types = context.SyntaxProvider.CreateSyntaxProvider(
            static (n, _) => n is PropertyDeclarationSyntax { IsMissing: false, Parent: not null },
            static (n, _) =>
            {
                var type = n.Node.Parent;

                if (type is not TypeDeclarationSyntax t)
                    return (IType?)null;
                
                return new TypeSymbol((ITypeSymbol)n.SemanticModel.GetDeclaredSymbol(type)!, t.IsPartial());
            });

        context.RegisterSourceOutput(types.Collect(), static (context, types) =>
        {
            var log = new CompilationLogProvider(context);
            var builder = new ModelInjectionTreeBuilder(log);

            var viewModels = types
                .WhereNotNull()
                .Distinct()
                .Where(t => t.Properties.Any(AttributeHelpers.HasBackingModelAttribute));
                
            foreach (var vm in viewModels)
            {
                try
                {
                    if (!builder.Build(vm, out var tree))
                        continue; //Errors will already have been written to log, we can ignore
                    var result = InjectionImplementationWriter.Generate(tree);
                    context.AddSource($"{vm.FullName}.g.cs", result);
                }
                catch (Exception e)
                {
                    new ErrorLogWriter(log).FatalError(vm, e);
                }
            }
        });
    }
}