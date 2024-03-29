﻿using Microsoft.CodeAnalysis;
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
        
        var returnKinds = context.SyntaxProvider.CreateSyntaxProvider(
            static (n, _) => n is TypeDeclarationSyntax,
            static (n, _) =>
            {
                var symbol = n.SemanticModel.GetDeclaredSymbol(n.Node);
                var syntax = (TypeDeclarationSyntax)n.Node;
                return new TypeSymbol((ITypeSymbol)symbol!, syntax.IsPartial());
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