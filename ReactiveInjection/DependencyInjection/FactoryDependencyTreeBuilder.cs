using ReactiveInjection.Framework;
using ReactiveInjection.Symbols;

// ReSharper disable InvertIf

// ReSharper disable MemberCanBeMadeStatic.Global

namespace ReactiveInjection.DependencyInjection;


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
        
        var factories = new List<(ViewModel ViewModel, IType[] Dependencies, IType[] SharedState)>();

        var attributes = factory.Attributes.Where(AttributeHelpers.IsReactiveFactoryAttribute).ToArray();

        //TODO: Error on duplicate types
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
            
            if (!GetConstructor(factory, vmType, out var constructor))
                continue;

            if (!ProcessParameters(factory,
                    constructor,
                    out var methodParams,
                    out var sharedState, 
                    out var services))
                continue;

            var vm = new ViewModel()
            {
                Type = vmType,
                MethodParams = methodParams,
                Constructor = constructor
            };
            factories.Add((vm, services, sharedState));
        }
        
        if (attributes.Length != factories.Count)
            return false;
        
        tree = new FactoryDependencyTree()
        {
            FactoryType = factory,
            ViewModels = factories.Select(f => f.ViewModel)
                .ToArray(),
            Services = factories
                .SelectMany(f => f.Dependencies)
                .Distinct()
                .ToArray(),
            SharedState = factories
                .SelectMany(f => f.SharedState)
                .Distinct()
                .ToArray(),
        };
        return true;
    }

    private bool GetConstructor(IType factory, IType viewModel, out IConstructor constructor)
    {
        //TODO: Support [ActivatorUtilitiesConstructor] attribute
        
        var constructors = viewModel.Constructors.ToList();
        if (constructors.Count == 1)
        {
            constructor = constructors[0];
            return true;
        }
        _log.MultipleConstructorsDefined(factory, viewModel);
        constructor = null!;
        return false;
    }

    private bool ProcessParameters(IType factoryType,
        IConstructor constructor,
        out IParameter[] methodParams,
        out IType[] sharedState,
        out IType[] services)
    {
        //Services marked with [FromServices]
        //State marked with [SharedState]
        
        //TODO: check services are reference types
        services = constructor.Parameters
            .Where(AttributeHelpers.HasFromServicesAttribute)
            .Select(p => p.Type)
            .ToArray();

        var sharedStateParams = constructor.Parameters
            .Where(AttributeHelpers.HasSharedStateAttribute)
            .ToArray();

        var validState = true;
        foreach (var param in sharedStateParams)
        {
            if (!param.Type.IsReferenceType)
            {
                _log.StateNotReferenceType(constructor, param);
                validState = false;
                continue;
            }
            if (param.Type.IsAbstract)
            {
                _log.StateIsAbstractType(constructor, param);
                validState = false;
                continue;
            }
            
            //Ensure it has a parameterless constructor
            if (!param.Type.Constructors.Any() || param.Type.Constructors.All(c => c.Parameters.Any()))
            {
                _log.StateHasNoParameterlessConstructor(constructor, param);
                validState = false;
                continue;
            }
        }

        sharedState = sharedStateParams.Select(p => p.Type).ToArray();

        methodParams = constructor.Parameters
            .Where(p => !AttributeHelpers.HasFromServicesAttribute(p)
                        && !AttributeHelpers.HasSharedStateAttribute(p)
                        && !p.Type.Equals(factoryType))
            .ToArray();
        
        return validState;
    }
}