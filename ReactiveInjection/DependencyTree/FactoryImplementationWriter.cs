using System.CodeDom.Compiler;
using System.Diagnostics;
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

        var w = new IndentedWriter();
        WriteFileHeader(w);

        w.WriteLine("#nullable disable");
        
        w.WriteLine($"namespace {tree.FactoryType.Namespace}");
        w.WriteLine('{');
        w.Push();
        
        //TODO: Get access level (public, internal etc) for type, methods etc

        w.WriteLine(DebuggerNonUserCode);
        w.WriteLine(GeneratedCode);
        w.WriteLine($"partial class {tree.FactoryType.Name}");
        w.WriteLine('{');
        w.Push();
        
        //Create readonly fields for services
        foreach (var svc in tree.Services)
            w.WriteLine($"private readonly {svc.CSharpName} {GetName(svc)}; //Dependency injected service");
        
        w.WriteLine();
        
        //Generate DI constructor
        w.Write($"public {tree.FactoryType.Name}(");
        WriteParameters(w, tree.Services, t => $"{t.CSharpName} {GetName(t)}");
        w.WriteRawLine(')');
        w.WriteLine('{');
        w.Push();
        foreach (var svc in tree.Services)
            w.WriteLine($"this.{GetName(svc)} = {GetName(svc)};");
        w.Pop();
        w.WriteLine('}');
        
        w.WriteLine();
        
        //Create readonly fields for shared state
        foreach (var state in tree.SharedState)
            w.WriteLine(
                $"private readonly {state.CSharpName} {GetName(state)} = new {state.CSharpName}(); //Shared state");
        
        foreach (var factory in tree.FactoryMethods)
        {
            w.WriteLine();
            
            w.Write($"public partial {factory.ReturnType.CSharpName} {factory.Method.Name}(");
            //We use the type field name as string, for reference below
            WriteParameters(w, 
                factory.Method.GetParameters(), 
                p => $"{p.Type.FullName} {p.Name}");
            
            w.WriteRawLine(')');
            w.WriteLine('{');
            
            w.Push();

            w.Write($"return new {factory.ReturnType.CSharpName}(");
            
            WriteParameters(w, 
                factory.Constructor.GetParameters(), 
                param =>
                {
                    //If is wrapping factory type, then use 'this'
                    if (param.Type.Equals(tree.FactoryType))
                        return "this";
                    
                    //Comes from method parameter?
                    var methodParam = factory.Method.GetParameters().FirstOrDefault(p => p.Name == param.Name);
                    
                    if (methodParam != null)
                        return methodParam.Name;
                    
                    //Otherwise we use the version from 'this' (either shared state or injected)
                    return $"this.{GetName(param.Type)}";
                });

            w.WriteRawLine(");");
            w.Pop();
            w.WriteLine('}');
        }
        
        //sb.Pop();
        w.PopThenWriteLine("}"); //End of type declaration
        w.PopThenWriteLine("}"); //End of namespace declaration

        return w.ToString();
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
    public static string GetName(IType type)
    {
        var str = type.FullName.Replace("global::", string.Empty);

        if (string.IsNullOrEmpty(str))
            throw new Exception("Full name is empty");

        //Convert Pascal.Case to camelCase

        var sb = new StringBuilder(str.Length);
        sb.Append(char.ToLowerInvariant(str[0]));
        for (var i = 1; i < str.Length; i++)
        {
            var c = str[i];
            switch (c)
            {
                case '<':
                    sb.Append('_');
                    break;
                case '[':
                    sb.Append("Array");
                    break;
                case '>' or '.' or ']':
                    //Do nothing
                    break;
                default:
                    sb.Append(c); //Already in pascalcase
                    break;
            }
        }

        return sb.ToString();
    }

    private static void WriteFileHeader(IndentedWriter writer)
    {
        writer.WriteLine("//This file was automatically generated by the ReactiveInjection source generator.");
        writer.WriteLine("//Do not edit this file manually since it will be automatically overwritten.");
    }
    
    private static readonly string DebuggerNonUserCode = $"[global::{typeof(DebuggerNonUserCodeAttribute).FullName}]";
    
    private static readonly string GeneratedCode = $"[global::{typeof(GeneratedCodeAttribute).FullName}(\"ReactiveInjection\", \"{Version}\")]";

    private static Version Version => typeof(FactoryImplementationWriter).Assembly.GetName().Version;
}