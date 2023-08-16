using System.Collections.Immutable;
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
                var isPartial = ((TypeDeclarationSyntax)n.Node).Modifiers
                    .Any(t => t.IsKind(SyntaxKind.PartialKeyword));
                return new TypeSymbol((ITypeSymbol)n.SemanticModel.GetDeclaredSymbol(n.Node)!, isPartial);
            });

        // var transformed = returnKinds
        //     .Select(static (t, _) => new TypeSymbol(t.Type, t.IsPartial))
        //     .Where(t => t.GetAttributes().Any(Attributes.IsReactiveFactoryAttribute));

        var transformed = returnKinds
            .Where(t => t.GetAttributes().Any(Attributes.IsReactiveFactoryAttribute));

        context.RegisterSourceOutput(transformed.Collect(), static (context, factories) =>
        {
            var log = new CompilationLogProvider(context);
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
                    context.AddSource($"{name}.g.cs", result);
                }
                catch (Exception e)
                {
                    log.WriteError(Location.None,
                        "RI1000",
                        "Unexpected error generating ViewModel factory implementation",
                        "An unexpected error occurred generating the view model factory '{0}': {1}: {2}",
                        factory,
                        e.GetType(),
                        e.Message);
                }
            }
        });
    }

    // private static void Handle(Compilation compilation, IEnumerable<TypeDeclarationSyntax> types, CancellationToken ct)
    // {
    //     const string factoryAttributeName = "ReactiveInjection.ReactiveFactoryAttribute";
    //     var attribute = compilation.GetTypeByMetadataName(factoryAttributeName)
    //                     ?? throw new Exception($"Unable to get type {factoryAttributeName}");
    //     foreach (var typeDeclaration in types)
    //     {
    //         ct.ThrowIfCancellationRequested();
    //
    //         var semanticModel = compilation.GetSemanticModel(typeDeclaration.SyntaxTree);
    //         var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration);
    //         if (typeSymbol == null)
    //             throw new Exception();
    //
    //         var attributes = typeSymbol.GetAttributes()
    //             .Where(attr => attr.AttributeClass != null
    //                            && attr.AttributeClass.Equals(attribute, SymbolEqualityComparer.Default));
    //         foreach (var attr in attributes)
    //         {
    //             if (attr.ConstructorArguments.Length == 0) 
    //                 throw new Exception("Attribute has no constructor arguments");
    //
    //             var arg = attr.ConstructorArguments[0];
    //             if (arg.Value is not ITypeSymbol vmType)
    //                 throw new Exception("Invalid constructor argument");
    //
    //             var type = new TypeSymbol(vmType);
    //         }
    //     }
    // }
}