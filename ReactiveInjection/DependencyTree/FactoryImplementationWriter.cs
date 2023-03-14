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

        var sb = new IndentedWriter();

        sb.WriteLine("#nullable disable");
        
        sb.WriteLine($"namespace {tree.FactoryType.Namespace}");
        sb.WriteLine('{');
        sb.Push();
        
        //TODO: Get access level (public, internal etc) for type, methods etc
        
        sb.WriteLine($"partial class {tree.FactoryType.Name}");
        sb.WriteLine('{');
        sb.Push();
        
        //Create readonly fields for services
        foreach (var svc in tree.Services)
            sb.WriteLine($"private readonly {svc.CSharpName} {GetName(svc)}; //Dependency injected service");
        
        sb.WriteLine();

        //Generate DI constructor
        sb.Write($"public {tree.FactoryType.Name}(");
        WriteParameters(sb, tree.Services, t => $"{t.CSharpName} {GetName(t)}");
        sb.WriteRawLine(')');
        sb.WriteLine('{');
        sb.Push();
        foreach (var svc in tree.Services)
            sb.WriteLine($"this.{GetName(svc)} = {GetName(svc)};");
        sb.Pop();
        sb.WriteLine('}');
        
        sb.WriteLine();
        
        //Create readonly fields for shared state
        foreach (var state in tree.SharedState)
            sb.WriteLine(
                $"private readonly {state.CSharpName} {GetName(state)} = new {state.CSharpName}(); //Shared state");
        
        foreach (var factory in tree.FactoryMethods)
        {
            sb.WriteLine();
            
            sb.Write($"public partial {factory.ReturnType.CSharpName} {factory.Method.Name}(");
            //We use the type field name as string, for reference below
            WriteParameters(sb, 
                factory.Method.GetParameters(), 
                p => $"{p.Type.FullName} {p.Name}");
            
            sb.WriteRawLine(')');
            sb.WriteLine('{');
            
            sb.Push();

            sb.Write($"return new {factory.ReturnType.CSharpName}(");
            
            WriteParameters(sb, 
                factory.Constructor.GetParameters(), 
                param =>
                {
                    //If is wrapping factory type, then use 'this'
                    if (param.Type.Equals(tree.FactoryType))
                        return "this";

                    //Comes from method parameter?
                    var methodParam = factory.Method.GetParameters().FirstOrDefault(p => p.Type.Equals(param.Type));
                    if (methodParam != null)
                        return methodParam.Name;
                    
                    //Otherwise we use the version from 'this' (either shared state or injected)
                    return $"this.{GetName(param.Type)}";
                });

            sb.WriteRawLine(");");
            sb.Pop();
            sb.WriteLine('}');
        }
        
        //sb.Pop();
        sb.PopThenWriteLine("}"); //End of type declaration
        sb.PopThenWriteLine("}"); //End of namespace declaration

        return sb.ToString();
    }

    private static void WriteParameters<T>(IndentedWriter writer,
        IReadOnlyCollection<T> parameters,
        Func<T, string> toString)
    {
        if (parameters.Count == 0)
            return;
        foreach (var t in parameters)
            writer.WriteRaw($"{toString(t)}, ");
        writer.TrimEnd(2); //Remove trailing ', '
    }

    /// <summary>
    /// Returns a name for a type suitable for
    /// use as a field name or filename
    /// </summary>
    public static string GetName(IType type) =>
        type.CSharpName
            .Replace("global::", "")
            .Replace('.', '_')
            .Replace('<', '_')
            .Replace(">", "");
}