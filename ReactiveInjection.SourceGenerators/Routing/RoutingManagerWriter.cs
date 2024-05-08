using ReactiveInjection.SourceGenerators.Framework;
using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.SourceGenerators.Routing;

internal class RoutingManagerWriter
{
    private readonly IndentedWriter _writer;
    private readonly RoutingTree _tree;

    private readonly Dictionary<IType, string> _services;

    private RoutingManagerWriter(IndentedWriter writer, RoutingTree tree)
    {
        _writer = writer;
        _tree = tree;

        _services = Enumerable.Range(0, tree.Services.Count)
            .ToDictionary(i => tree.Services[i], i => $"_service{i}");
    }
    
    public static string Generate(RoutingTree tree)
    {
        var writer = new IndentedWriter();
        new RoutingManagerWriter(writer, tree).GenInternal();
        return writer.ToString();
    }

    public const string ClassName = "ReactiveRoutingService";

    private void GenInternal()
    {
        _writer.WriteFileHeader("enable");
        
        _writer.WriteLine($"namespace {_tree.Assembly.Name}");
        _writer.WriteLineThenPush('{');

        _writer.WriteAttributes();
        
        const string @base = $"global::ReactiveInjection.Routing.ReactiveRoutingServiceBase<{ClassName}>";
        _writer.WriteLine($"public class {ClassName} : {@base}");
        _writer.WriteLineThenPush('{');

        WriteConstructor();
        WriteLoaders();

        _writer.PopThenWriteLine('}');
        _writer.PopThenWriteLine('}');
    }

    private void WriteConstructor()
    {
        const string handler = "global::ReactiveInjection.Routing.IReactiveRouterHandler";
        for (var i = 0; i < _tree.Services.Count; i++)
            _writer.WriteLine($"private readonly {_tree.Services[i].CSharpName} _service{i};");

        _writer.WriteLine();
        _writer.Write($"public {ClassName}({handler} handler");
        if (_tree.Services.Count > 0)
        {
            _writer.WriteRaw(", ");
            _writer.WriteParameters(_tree.Services, (t, i) => $"{t.CSharpName} service{i}");
        }

        _writer.WriteRawLine(")");
        
        _writer.Push();
        _writer.WriteLine(": base(handler, Loaders)");
        _writer.Pop();
        
        _writer.WriteLineThenPush('{');
        for (var i = 0; i < _tree.Services.Count; i++)
            _writer.WriteLine($"this._service{i} = service{i};");
        _writer.PopThenWriteLine('}');
        _writer.WriteLine();
    }

    private void WriteLoaders()
    {
        const string loader = $"global::ReactiveInjection.Routing.ReactiveViewModelLoader<{ClassName}>";
        
        _writer.WriteLine($"private static readonly {loader}[] Loaders = new {loader}[]");
        _writer.WriteLineThenPush('{');
        foreach (var route in _tree.Routes)
        {
            _writer.WriteLineThenPush($"new {loader}(");
            _writer.WriteLine($"typeof({route.ViewModel.CSharpName}),");
            _writer.WriteLine($"@\"{route.Method.Name}\",");
            _writer.WriteLine($"@\"{route.RouteTemplate}\",");
            _writer.WriteLine($"{GenMatchMethod(route)},");
            _writer.WriteLine($"{GenLoadMethod(route)}),");
            _writer.Pop();
        }
        _writer.PopThenWriteLine("};");
    }

    private static string GenMatchMethod(ViewModelLoaderRoute route)
    {
        //Param name is a
        //looks like r => r.Length == route.Segments.Count && int.TryParse(r[0], out _)

        var clauses = new List<string>()
        {
            $"r.Length == {route.Segments.Count}"
        };
        for (var i = 0; i < route.Segments.Count; i++)
        {
            switch (route.Segments[i])
            {
                case string str:
                    //Validate that the string segment matches
                    clauses.Add($"r[{i}].Equals(@\"{str}\", StringComparison.OrdinalIgnoreCase)");
                    break;
                case RouteParameter p:
                    //Validate that the parameter can be parsed
                    if (p.Type == RouteParameterType.String) break;
                    clauses.Add($"{p.CSharpType}.TryParse(r[{i}], out var _)");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        return $"r => {string.Join(" && ", clauses)}";
    }

    private string GenLoadMethod(ViewModelLoaderRoute route)
    {
        //Looks like async (s, r, ct) => (object)(await ViewModel1.Load(r[0], int.Parse(r[1]), s._service0, ct))
        
        var parameters = new List<string>();
        foreach (var p in route.Method.Parameters)
        {
            if (AttributeHelpers.HasFromServicesAttribute(p))
            {
                parameters.Add($"s.{_services[p.Type]}");
                continue;
            }

            if (p.Type.IsCancellationToken())
            {
                parameters.Add("ct");
                continue;
            }
            
            var rp = route.Segments.OfType<RouteParameter>().Single(x => x.NameEquals(p.Name));
            var value = $"r[{rp.Index}]";
            parameters.Add(rp.Type switch
            {
                RouteParameterType.String => value,
                _ => $"{rp.CSharpType}.Parse({value})"
            });
        }

        var call = $"{route.ViewModel.CSharpName}.{route.Method.Name}({string.Join(", ", parameters)})";
        return $"async (s, r, ct) => (object)(await {call})";
    }
}