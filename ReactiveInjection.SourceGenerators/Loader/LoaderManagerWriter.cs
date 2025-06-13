using System.ComponentModel;
using System.Globalization;
using ReactiveInjection.SourceGenerators.Framework;
using ReactiveInjection.SourceGenerators.Symbols;
// ReSharper disable SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault

namespace ReactiveInjection.SourceGenerators.Loader;

internal class LoaderManagerWriter
{
    private readonly IndentedWriter _writer;
    private readonly LoaderTree _tree;

    private readonly Dictionary<IType, string> _services;

    private LoaderManagerWriter(IndentedWriter writer, LoaderTree tree)
    {
        _writer = writer;
        _tree = tree;

        _services = Enumerable.Range(0, tree.Services.Count)
            .ToDictionary(i => tree.Services[i], i => $"_service{i}");
    }
    
    public static string Generate(LoaderTree tree)
    {
        var writer = new IndentedWriter();
        new LoaderManagerWriter(writer, tree).GenInternal();
        return writer.ToString();
    }

    public const string Namespace = "ReactiveInjection.Loader";
    public const string ClassName = "ReactiveLoaderManager";

    private void GenInternal()
    {
        _writer.WriteFileHeader("disable");
        
        _writer.WriteLine($"namespace {Namespace}");
        _writer.WriteLineThenPush('{');
        
        const string @interface = "global::ReactiveInjection.Loader.IReactiveLoaderManager";
        const string @base = "global::ReactiveInjection.Loader.ReactiveLoaderManagerBase";
        _writer.WriteAttributes(EditorBrowsableState.Never);
        _writer.WriteLine($"internal class {ClassName} : {@base}, {@interface}");
        _writer.WriteLineThenPush('{');

        WriteConstructor();
        WriteLoaders();

        _writer.WriteLine();
        
        // //Implement interface.
        // //Loaders is a static field for performance reasons (avoid creating a new array on each instance)
        // //Manager is accessed via first parameter of load method
        // const string list = "global::System.Collections.Generic.IReadOnlyList";
        // const string loader = "global::ReactiveInjection.Loader.ReactiveViewModelLoader";
        // _writer.WriteLine($"{list}<{loader}> {@interface}.Loaders => Loaders;");
        
        _writer.PopThenWriteLine('}');
        _writer.PopThenWriteLine('}');
    }

    private void WriteConstructor()
    {
        for (var i = 0; i < _tree.Services.Count; i++)
            _writer.WriteLine($"private readonly {_tree.Services[i].CSharpName} _service{i};");

        _writer.WriteLine();
        _writer.Write($"public {ClassName}(");
        if (_tree.Services.Count > 0)
        {
            _writer.WriteParameters(_tree.Services, (t, i) => $"{t.CSharpName} service{i}");
        }

        _writer.WriteRawLine(")");
        
        _writer.WriteLineThenPush('{');
        for (var i = 0; i < _tree.Services.Count; i++)
            _writer.WriteLine($"this._service{i} = service{i};");
        _writer.PopThenWriteLine('}');
        _writer.WriteLine();
    }

    private void WriteLoaders()
    {
        const string list = "global::System.Collections.Generic.IReadOnlyList";
        const string @interface = "global::ReactiveInjection.Loader.IReactiveViewModelLoader";
        const string loader = "global::ReactiveInjection.Loader.ReactiveViewModelLoader";
        
        _writer.WriteLine($"public {list}<{@interface}> Loaders => new {@interface}[]");
        _writer.WriteLineThenPush('{');
        foreach (var route in _tree.Routes)
        {
            var methodParams = route.Method.Parameters.Select(p => $"typeof({p.Type.CSharpName})");
            
            _writer.WriteLineThenPush($"new {loader}<{ClassName}>(");
            _writer.WriteLine($"typeof({route.ViewModel.CSharpName}),");
            _writer.WriteLine($"{route.Method.Name.ToStringLiteral()},");
            _writer.WriteLine($"new Type[] {{ {string.Join(", ", methodParams)} }},");
            _writer.WriteLine($"{route.RouteTemplate.ToStringLiteral()},");
            _writer.WriteLine("this,");
            _writer.WriteLine($"{GenMatchMethod(route)},");
            _writer.WriteLine($"{GenLoadMethod(route)}),");
            _writer.Pop();
        }
        _writer.PopThenWriteLine("};");
    }

    private static string GenMatchMethod(ViewModelLoaderRoute route)
    {
        //parameter is (string[] segments)
        
        //looks like s  => s.Length == route.Segments.Count && int.TryParse(s[0], out _)

        var length = $"s.Length == {route.Segments.Count}";
        var clauses = new List<string>()
        {
            route.Segments.LastOrDefault() is RouteSegmentParameter { Optional: true }
                //Last parameter is optional, so length can be 1 less
                ? $"({length} || s.Length == {route.Segments.Count - 1})"
                : length
        };
        
        for (var i = 0; i < route.Segments.Count; i++)
        {
            switch (route.Segments[i])
            {
                case string str:
                    //Validate that the string segment matches
                    clauses.Add($"s[{i}].Equals({str.ToStringLiteral()}, StringComparison.OrdinalIgnoreCase)");
                    break;
                case RouteSegmentParameter p:
                    //Validate that the parameter value is of the correct type
                    switch (p.Type)
                    {
                        case RouteParameterType.String:
                            break; //No validation needed
                        case RouteParameterType.Parsable or RouteParameterType.Primitive:
                            //For parsable types, we call TryParse to validate
                            
                            var tryParse = $"{p.TypeSymbol!.CSharpName}.TryParse(s[{i}]";
                            if (p.Type == RouteParameterType.Parsable)
                                //Invariant culture for IParsable
                                tryParse += $", {CultureInfoInvariant}";
                            tryParse += ", out _)";
                            if (p.Optional)
                                //If optional, segment may not be passed
                                tryParse = $"(s.Length <= {i} || string.IsNullOrEmpty(s[{i}]) || {tryParse})";
                            clauses.Add(tryParse);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        return $"static s => {string.Join(" && ", clauses)}";
    }

    private string GenLoadMethod(ViewModelLoaderRoute route)
    {
        const string taskFromResult = "global::System.Threading.Tasks.Task.FromResult";
        
        //Parameters are (TManager manager, string[] segments, NameValueCollection query, object[] parameters, CancellationToken ct)
        //Looks like async (m, r, q, p, ct) =>
        //(object)(await ViewModel1.Load(s[0], int.Parse(s[1]), q.GetValues("p")?.FirstOrDefault(), m._service0, o as OwnerType, ct))

        var parameters = new List<string>();
        foreach (var p in route.Method.Parameters)
        {
            if (AttributeHelpers.HasFromServicesAttribute(p))
            {
                parameters.Add($"m.{_services[p.Type]}");
                continue;
            }

            if (AttributeHelpers.HasFromParametersAttribute(p))
            {
                var getParam = $"GetParameter<{p.Type.CSharpName}>(p, {p.Name.ToStringLiteral()})";
                parameters.Add(getParam);
                continue;
            }

            if (p.Type.IsCancellationToken())
            {
                parameters.Add("ct");
                continue;
            }

            var query = p.Attributes.SingleOrDefault(AttributeHelpers.IsFromLoaderQueryAttribute);
            if (query != null)
            {
                var name = query.StringParameterNullable ?? p.Name;
                parameters.Add($"q.GetValues({name.ToStringLiteral()})?.FirstOrDefault()");
                continue;
            }
            
            var rp = route.Segments
                .OfType<RouteSegmentParameter>()
                .Single(x => x.NameEquals(p.Name));
            var value = $"r[{rp.Index}]";
            value = rp.Type switch
            {
                RouteParameterType.String => value,
                RouteParameterType.Primitive => $"{p.Type.CSharpName}.Parse({value})",
                RouteParameterType.Parsable => $"{rp.TypeSymbol!.CSharpName}.Parse({value}, {CultureInfoInvariant})",
                _ => throw new ArgumentOutOfRangeException()
            };
            if (rp.Optional)
                //If optional, segment may not be passed. In that case, pass default(TParamType)
                value = $"r.Length <= {rp.Index} || string.IsNullOrEmpty(r[{rp.Index}]) ? default({p.Type.CSharpName}) : {value}";
            parameters.Add(value);
        }

        var call = $"{route.ViewModel.CSharpName}.{route.Method.Name}({string.Join(", ", parameters)})";

        return route.Method.ReturnType!.IsTask(out _) 
            ? $"static async (m, r, q, p, ct) => (object)(await {call})" 
            : $"static (m, r, q, p, ct) => {taskFromResult}<object>({call})";
    }

    private static readonly string CultureInfoInvariant =
        $"global::{typeof(CultureInfo).FullName}.{nameof(CultureInfo.InvariantCulture)}";
}