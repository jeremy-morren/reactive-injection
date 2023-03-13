using System.Text;
using ReactiveInjection.Tokens;
// ReSharper disable MemberCanBeMadeStatic.Global

namespace ReactiveInjection.DependencyTree;

internal class FactoryImplementationWriter
{
    private readonly IErrorLog _log;

    public FactoryImplementationWriter(IErrorLog log) => _log = log;

    public string GenerateCSharp(FactoryDependencyTree tree)
    {
        //We are aiming for early C# here
        //For simplicity, we end up with the opening brace on the same line

        var sb = new IndentedWriter();

        sb.WriteLineAndPushIndent($"namespace {tree.FactoryType.Namespace} {{");
        
        //TODO: Get access level (public, internal etc) for type, methods etc
        
        sb.WriteLineAndPushIndent($"public partial class {tree.FactoryType.Name} {{");
        
        //Create readonly fields for services
        foreach (var svc in tree.Services)
            sb.WriteLine($"private readonly {svc.CSharpName} {GetFieldName(svc)}; //Dependency injected service");

        //Generate DI constructor
        sb.Write($"public {tree.FactoryType.Name}(");
        WriteParameters(sb, tree.Services, t => $"{t.CSharpName} {GetFieldName(t)}");
        sb.WriteLineWithoutIndentAndPushIndent(") {");
        foreach (var svc in tree.Services)
            sb.WriteLine($"this.{GetFieldName(svc)} = {GetFieldName(svc)};");
        sb.PopIndentAndWriteLine("}");
        
        //Create readonly fields for shared state
        foreach (var state in tree.SharedState)
            sb.WriteLine(
                $"private readonly {state.CSharpName} {GetFieldName(state)} = new {state.CSharpName}(); //Shared state");

        foreach (var factory in tree.FactoryMethods)
        {
            sb.Write($"public {factory.ReturnType.CSharpName} {factory.Method.Name}(");
            //We use the type field name as string, for reference below
            WriteParameters(sb, factory.Method.GetParameters(), p => 
                $"{p.ParameterType.FullName} {GetFieldName(p.ParameterType)}");
            sb.WriteLineWithoutIndentAndPushIndent(") {");

            sb.Write($"return new {factory.ReturnType.CSharpName}(");
            
            WriteParameters(sb, factory.Constructor.GetParameters(), param =>
            {
                //If is wrapping factory type, then use 'this'
                if (param.ParameterType.Equals(tree.FactoryType))
                    return "this";
                
                //Comes from method parameter?
                if (factory.Method.GetParameters().Any(p => p.ParameterType.Equals(p.ParameterType)))
                    return GetFieldName(param.ParameterType); //Use local variable (from parameter list)
                
                //Otherwise we use the version from 'this' (either shared state or injected)
                return $"this.{GetFieldName(param.ParameterType)}";
            });

            sb.WriteRawLine(");");
            sb.PopIndentAndWriteLine("}");
        }

        sb.PopIndentAndWriteLine("}"); //End of type declaration
        sb.PopIndentAndWriteLine("}"); //End of namespace declaration

        return sb.ToString();
    }

    private static void WriteParameters<T>(IndentedWriter writer,
        IReadOnlyCollection<T> parameters,
        Func<T, string> toString)
    {
        if (parameters.Count == 0) return;
        writer.WriteLineAndPushIndent(string.Empty);
        foreach (var t in parameters)
            writer.WriteLine($"{toString(t)},");
        writer.TrimEnd(1 + Environment.NewLine.Length); //Remove trailing ',{Newline}'
    }

    private static string GetFieldName(IType type) =>
        type.CSharpName
            .Replace("global::", "")
            .Replace('.', '_')
            .Replace('<', '_')
            .Replace(">", "");
}