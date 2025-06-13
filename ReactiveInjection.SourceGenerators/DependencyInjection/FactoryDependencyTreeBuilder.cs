using ReactiveInjection.SourceGenerators.DependencyInjection.Models;
using ReactiveInjection.SourceGenerators.Framework;
using ReactiveInjection.SourceGenerators.Symbols;

// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable InvertIf
// ReSharper disable MemberCanBeMadeStatic.Global

namespace ReactiveInjection.SourceGenerators.DependencyInjection;


internal class FactoryDependencyTreeBuilder
{
    private readonly ErrorLogWriter _log;

    public FactoryDependencyTreeBuilder(IErrorLog log)
    {
        _log = new ErrorLogWriter(log);
    }

    public bool Build(IType factory, out FactoryDependencyTree tree)
    {
        tree = null!;
        
        if (!factory.IsPartial)
        {
            _log.FactoryIsNotPartial(factory);
            return false;
        }
        
        if (factory.IsGenericType)
        {
            _log.FactoryIsGenericType(factory);
            return false;
        }

        var viewModels = new List<(ViewModel ViewModel, List<IType> SharedState, List<IType> Services)>();

        var attributes = factory.Attributes.Where(AttributeHelpers.IsReactiveFactoryAttribute).ToArray();

        var duplicates = attributes.Duplicates(a => a.TypeParameter).ToList();

        if (duplicates.Count > 0)
        {
            foreach (var t in duplicates) 
                _log.DuplicateViewModel(factory, t);
            return false;
        }

        //TODO: Error on different types with same name
        
        foreach (var attribute in attributes)
        {
            var vmType = attribute.TypeParameter;
            if (!vmType.IsReferenceType)
            {
                _log.ViewModelNotReferenceType(factory, vmType);
                continue;
            }

            if (vmType.IsAbstract)
            {
                _log.ViewModelIsAbstract(factory, vmType);
                continue;
            }

            //Add constructor method
            if (GetConstructor(factory, vmType, out var constructor)
                && ProcessMethod(factory, 
                    constructor.Parameters.ToList(),
                    out var methodParams,
                    out var sharedState,
                    out var services))
            {
                var vm = new ConstructorViewModel()
                {
                    Type = vmType,
                    MethodParams = methodParams,
                    Constructor = constructor
                };
                viewModels.Add((vm, sharedState, services));
            }

            //Add Load methods
            foreach (var method in vmType.Methods)
            {
                if (!method.IsPublic 
                    || !method.IsStatic 
                    || !method.Name.StartsWith("Load")
                    // Ignore methods with [LoaderRoute] attribute
                    || method.Attributes.Any(AttributeHelpers.IsLoaderRouteAttribute))
                    continue;
                if (method.ReturnType == null 
                    || !method.ReturnType.IsTask(out var arg) 
                    || !arg.Equals(vmType))
                {
                    _log.IncorrectLoaderSignature(method);
                    continue;
                }
                if (ProcessMethod(factory, 
                    method.Parameters.ToList(),
                    out methodParams,
                    out sharedState,
                    out services))
                {
                    var vm = new LoaderViewModel()
                    {
                        Type = vmType,
                        Method = method,
                        MethodParams = methodParams,
                    };
                    viewModels.Add((vm, sharedState, services));
                }
            }
        }
        
        tree = new FactoryDependencyTree()
        {
            FactoryType = factory,
            ViewModels = viewModels.Select(f => f.ViewModel).ToList(),
            Services = viewModels.SelectMany(f => f.Services).Distinct().ToList(),
            SharedState = viewModels.SelectMany(f => f.SharedState).Distinct().ToList(),
        };
        return true;
    }

    private bool GetConstructor(IType factory, IType viewModel, out IConstructor constructor)
    {
        //TODO: Support [ActivatorUtilitiesConstructor] attribute
        
        constructor = null!;
        var constructors = viewModel.Constructors.Where(c => c.IsPublic).ToList();
        switch (constructors.Count)
        {
            case 1:
                constructor = constructors[0];
                return true;
            case 0:
                return false;
            default:
                _log.MultipleConstructorsDefined(factory, viewModel);
                return false;
        }
    }

    private bool ProcessMethod(IType viewModel,
        List<IParameter> parameters,
        out IParameter[] methodParams,
        out List<IType> sharedState,
        out List<IType> services)
    {
        //Services marked with [FromServices]
        //State marked with [SharedState]
        
        //TODO: check services are reference types
        services = parameters
            .Where(AttributeHelpers.HasFromServicesAttribute)
            .Select(p => p.Type)
            .ToList();

        var sharedStateParams = parameters
            .Where(AttributeHelpers.HasSharedStateAttribute)
            .ToArray();

        var validState = true;
        foreach (var param in sharedStateParams)
        {
            if (!param.Type.IsReferenceType)
            {
                _log.StateNotReferenceType(viewModel, param);
                validState = false;
                continue;
            }
            if (param.Type.IsAbstract)
            {
                _log.StateIsAbstractType(viewModel, param);
                validState = false;
                continue;
            }
            
            //Ensure it has a parameterless constructor
            if (!param.Type.Constructors.Any() || param.Type.Constructors.All(c => c.Parameters.Any()))
            {
                _log.StateHasNoParameterlessConstructor(viewModel, param);
                validState = false;
                continue;
            }
        }

        sharedState = sharedStateParams.Select(p => p.Type).ToList();

        methodParams = parameters
            .Where(p => !AttributeHelpers.HasFromServicesAttribute(p)
                        && !AttributeHelpers.HasSharedStateAttribute(p)
                        && !p.Type.Equals(viewModel)
                        && !p.Type.IsCancellationToken())
            .ToArray();
        
        return validState;
    }
}