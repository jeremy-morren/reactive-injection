using JetBrains.Annotations;
using ReactiveInjection.SourceGenerators.DependencyInjection.Models;
using ReactiveInjection.SourceGenerators.Framework;
using ReactiveInjection.SourceGenerators.Symbols;

// ReSharper disable ConvertIfStatementToReturnStatement

namespace ReactiveInjection.SourceGenerators.DependencyInjection;

internal class FactoryImplementationWriter
{
    private readonly IndentedWriter _writer;
    private readonly FactoryDependencyTree _tree;

    /// <summary>
    /// Map of services to field names
    /// </summary>
    private readonly Dictionary<IType, string> _services;
    
    /// <summary>
    /// Map of shared state to field names
    /// </summary>
    private readonly Dictionary<IType, string> _sharedState;

    private FactoryImplementationWriter(IndentedWriter writer, FactoryDependencyTree tree)
    {
        _writer = writer;
        _tree = tree;
        
        _services = Enumerable.Range(0, _tree.Services.Count)
            .ToDictionary(i => _tree.Services[i], i => $"this._service{i}");
        
        _sharedState = Enumerable.Range(0, _tree.SharedState.Count)
            .ToDictionary(i => _tree.SharedState[i], i => $"this._state{i}");
    }

    public static string GenerateCSharp(FactoryDependencyTree tree)
    {
        var writer = new IndentedWriter();
        new FactoryImplementationWriter(writer, tree).Generate();
        return writer.ToString();
    }
    
    private void Generate()
    {
        _writer.WriteFileHeader("enable");
        
        //TODO: Handle generic parameter nullability
        _writer.WriteLine("#nullable disable warnings");

        _writer.WritePartialTypeDefinition(_tree.FactoryType);

        _writer.WriteLine($"private readonly ILogger _logger;");
        _writer.WriteLine();
        
        //Create readonly fields for services
        for (var i= 0; i < _tree.Services.Count; i++)
        {
            var type = _tree.Services[i];
            _writer.WriteLine($"private readonly {type.CSharpName} _service{i};");
        }
        
        _writer.WriteLine();
        
        //Create readonly fields for shared state
        for (var i = 0; i < _tree.SharedState.Count; i++)
        {
            var type = _tree.SharedState[i];
            _writer.WriteLine($"private readonly {type.CSharpName} _state{i} = new {type.CSharpName}();");
        }

        _writer.WriteLine();
        
        //Generate DI constructor
        _writer.WriteAttributes();
        _writer.Write($"public {_tree.FactoryType.Name}(");
        
        WriteParameters(_tree.Services, (t, i) => $"{t.CSharpName} service{i}");

        if (_tree.Services.Count > 0)
            _writer.WriteRaw(", ");
        _writer.WriteRawLine("ILoggerFactory? loggerFactory = null)");
        
        _writer.WriteLineThenPush('{');

        const string nullLoggerFactory = "global::Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance";
        _writer.WriteLine($"this._logger = (loggerFactory ?? {nullLoggerFactory}).CreateLogger(this.GetType());");
        _writer.WriteLine();
        
        for (var i = 0; i < _tree.Services.Count; i++)
            _writer.WriteLine($"this._service{i} = service{i};");
        _writer.PopThenWriteLine('}');
        
        //Write view model methods
        //Short name is method name
        foreach (var vm in _tree.ViewModels)
        {
            _writer.WriteLine();
            switch (vm)
            {
                case ConstructorViewModel c:
                    WriteConstructor(c);
                    break;
                case LoaderViewModel l:
                    WriteLoader(l);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        
        _writer.PopThenWriteLine("}"); //End of type declaration
        _writer.PopThenWriteLine("}"); //End of namespace declaration
    }

    private void WriteConstructor(ConstructorViewModel vm)
    {
        _writer.WriteAttributes();
        
        _writer.Write($"public {vm.Type.CSharpName} {vm.Type.Name}(");

        WriteMethodParametersDefinition(vm);
        
        _writer.WriteRawLine(')');
        _writer.WriteLineThenPush('{');
        
        WriteTryCatch(() =>
        {
            _writer.Write($"return new {vm.Type.CSharpName}(");

            WriteMethodParameters(vm.Constructor.Parameters);
        
            _writer.WriteRawLine(");");
        },
        () =>
        {
            LogError("Error constructing {ViewModel}", $"typeof({vm.Type.CSharpName})");
            _writer.WriteLine("throw;");
        });
        
        _writer.PopThenWriteLine('}');
    }

    private void WriteLoader(LoaderViewModel vm)
    {
        //Method is Observable.FromAsync(ct => Load(ct), scheduler: RxApp.MainThreadScheduler)
        
        const string scheduler = "global::ReactiveUI.RxApp.MainThreadScheduler";
        const string fromAsync = "global::System.Reactive.Linq.Observable.FromAsync";
        
        _writer.WriteAttributes();
        _writer.Write($"public global::System.IObservable<{vm.Type.CSharpName}> Load{vm.Type.Name}(");
        WriteMethodParametersDefinition(vm);
        
        _writer.WriteRawLine(')');
        _writer.WriteLineThenPush('{');
        
        _writer.WriteLine($"return {fromAsync}<{vm.Type.CSharpName}>(async (global::System.Threading.CancellationToken ct) =>");
        _writer.WriteLineThenPush('{');
        
        WriteTryCatch(() =>
        {
            _writer.Write($"return await {vm.Type.CSharpName}.{vm.Method.Name}(");
            WriteMethodParameters(vm.Method.Parameters);
            _writer.WriteRawLine(");");
        },
        () =>
        {
            LogError("Error calling loader {LoaderMethod} for {ViewModel}", 
                $"\"{vm.Method.Name}\"", $"typeof({vm.Type.CSharpName})");

            _writer.WriteLine("throw;");
        });
        
        _writer.PopThenWriteLine($"}}, scheduler: {scheduler});");
        
        _writer.PopThenWriteLine('}');
    }

    private void WriteMethodParametersDefinition(ViewModel vm)
    {
        WriteParameters(vm.MethodParams, 
            p =>
            {
                var nullable = p.Type.IsNullable ? "?" : null;
                return $"{p.Type.CSharpName}{nullable} {p.Name}";
            });
    }

    private void WriteMethodParameters(IEnumerable<IParameter> parameters)
    {
        WriteParameters(parameters, 
            param =>
            {
                //If is wrapping factory type, then use 'this'
                if (param.Type.Equals(_tree.FactoryType))
                    return "this";

                if (param.Type.IsCancellationToken())
                    return "ct";
                
                if (AttributeHelpers.HasFromServicesAttribute(param))
                    return _services[param.Type];

                if (AttributeHelpers.HasSharedStateAttribute(param))
                    return _sharedState[param.Type];
                
                //Comes from method parameter
                return param.Name;
            });
    }

    private void WriteParameters<T>(IEnumerable<T> parameters, Func<T, string> toString)
    {
        _writer.WriteRaw(string.Join(", ", parameters.Select(toString)));
    }
    
    private void WriteParameters<T>(IEnumerable<T> parameters, Func<T, int, string> toString)
    {
        _writer.WriteRaw(string.Join(", ", parameters.Select(toString)));
    }

    private void LogError([StructuredMessageTemplate] string message, params string[] args)
    {
        var argList = string.Join(", ", args);
        
        _writer.WriteLine($"_logger.LogError(ex, \"{message}\", args: new object[]{{{argList}}});");
    }

    private void WriteTryCatch(Action @try, Action @catch)
    {
        _writer.WriteLine("try");
        _writer.WriteLineThenPush('{');
        @try();
        _writer.PopThenWriteLine('}');
        _writer.WriteLine("catch (Exception ex)");
        _writer.WriteLineThenPush('{');
        @catch();
        _writer.PopThenWriteLine('}');
    }
}