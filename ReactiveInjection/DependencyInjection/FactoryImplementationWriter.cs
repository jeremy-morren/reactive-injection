using ReactiveInjection.Framework;

// ReSharper disable ConvertIfStatementToReturnStatement

namespace ReactiveInjection.DependencyInjection;

internal static class FactoryImplementationWriter
{
    public static string GenerateCSharp(FactoryDependencyTree tree)
    {
        var w = new IndentedWriter();
        w.WriteFileHeader("enable");
        
        //TODO: Handle generic parameter nullability
        w.WriteLine("#pragma warning disable CS8620"); //Parameter nullability warning

        w.WritePartialTypeDefinition(tree.FactoryType);

        var services = new Dictionary<string, string>(); //Map of service type to service field name
        
        //Create readonly fields for services
        for (var i= 0; i < tree.Services.Length; i++)
        {
            var type = tree.Services[i];
            services.Add(type.CSharpName, $"this._service{i}");
            w.WriteLine($"private readonly {type.CSharpName} _service{i};");
        }
        
        w.WriteLine();
        
        //Create readonly fields for shared state
        var sharedState = new Dictionary<string, string>();
        for (var i = 0; i < tree.SharedState.Length; i++)
        {
            var type = tree.SharedState[i];
            sharedState.Add(type.CSharpName, $"this._state{i}");
            w.WriteLine($"private readonly {type.CSharpName} _state{i} = new {type.CSharpName}();");
        }

        w.WriteLine();
        
        //Generate DI constructor
        w.WriteMethodAttributes();
        w.Write($"public {tree.FactoryType.Name}(");
        WriteParameters(w, tree.Services, (t, i) => $"{t.CSharpName} service{i}");
        w.WriteRawLine(')');
        w.WriteLineThenPush('{');
        for (var i = 0; i < tree.Services.Length; i++)
            w.WriteLine($"this._service{i} = service{i};");
        w.PopThenWriteLine('}');
        
        //Write view model methods
        //Short name is method name
        foreach (var vm in tree.ViewModels)
        {
            w.WriteLine();
            
            w.WriteMethodAttributes();
            w.WriteGeneratedCodeAttribute();
            w.Write($"public {vm.Type.CSharpName} {vm.Type.Name}(");
            
            WriteParameters(w, 
                vm.MethodParams,
                p =>
                {
                    var nullable = p.Type.IsNullable ? "?" : null;
                    return $"{p.Type.CSharpName}{nullable} {p.Name}";
                });
            
            w.WriteRawLine(')');
            w.WriteLineThenPush('{');
            
            w.Write($"return new {vm.Type.CSharpName}(");
            
            WriteParameters(w, 
                vm.Constructor.Parameters, 
                param =>
                {
                    //If is wrapping factory type, then use 'this'
                    if (param.Type.Equals(tree.FactoryType))
                        return "this";
                    
                    if (AttributeHelpers.HasFromServicesAttribute(param))
                        return services[param.Type.CSharpName];

                    if (AttributeHelpers.HasSharedStateAttribute(param))
                        return sharedState[param.Type.CSharpName];
                    
                    //Comes from method parameter
                    return param.Name;
                });
            
            w.WriteRawLineAndPop(");");
            w.WriteLine('}');
        }
        
        //sb.Pop();
        w.PopThenWriteLine("}"); //End of type declaration
        w.PopThenWriteLine("}"); //End of namespace declaration
        
        return w.ToString();
    }

    private static void WriteParameters<T>(IndentedWriter writer,
        IEnumerable<T> parameters,
        Func<T, string> toString)
    {
        writer.WriteRaw(string.Join(", ", parameters.Select(toString)));
    }
    
    private static void WriteParameters<T>(IndentedWriter writer,
        IEnumerable<T> parameters,
        Func<T, int, string> toString)
    {
        writer.WriteRaw(string.Join(", ", parameters.Select(toString)));
    }
}