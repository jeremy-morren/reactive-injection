using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveInjection.Tokens;

namespace ReactiveInjection;

[Generator(LanguageNames.CSharp)]
public class ReactiveFactoryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var returnKinds = context.SyntaxProvider.CreateSyntaxProvider(
            static (n, _) => n is TypeDeclarationSyntax,
            static (n, _) => (ITypeSymbol)n.SemanticModel.GetDeclaredSymbol(n.Node)!);

        var transformed = returnKinds.Select(static (m, ct) =>
        {
            return new
            {
                Namespace = m.ContainingNamespace.Name,
                Type = m.ContainingType.Name,
                Method = m.Name,
                Attributes = ToString(m.GetAttributes()
                    .Select(a => new { a.AttributeClass?.Name }.ToString()))
            };
        });

        var collected = transformed.Collect();

        context.RegisterSourceOutput(collected, static (sourceProductionContext, factories) =>
        {
            var sb = new StringBuilder();

            sb.AppendLine(
                "namespace Generated {\npublic static class MethodHelpers {\npublic static void LogMethods(System.Action<string> log) {\n");

            foreach (var factory in factories)
            {
                sb.AppendLine($"log(\"{factory}\");");
            }

            sb.AppendLine("} } }");

            sourceProductionContext.AddSource("MethodNames.cs", sb.ToString());
        });
    }


    private record Factory(string ContainingNamespace, string ParentClassName, string MethodName, bool ShouldProcess);

    private static string ToString<T>(IEnumerable<T> input)
    {
        var values = input.Select(v => v?.ToString());

        return $"[ {string.Join(", ", values)} ]";
    }
}