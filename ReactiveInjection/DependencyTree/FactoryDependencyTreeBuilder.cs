using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveInjection.Tokens;
using ReactiveInjection.Tokens.Generator;
// ReSharper disable InvertIf

// ReSharper disable MemberCanBeMadeStatic.Global

namespace ReactiveInjection.DependencyTree;


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
        
        var factories = new List<(ViewModel ViewModel, IType[] Dependencies, IType[] SharedState)>();

        var attributes = factory.GetAttributes().Where(Attributes.IsReactiveFactoryAttribute).ToArray();

        //TODO: Error on duplicate types
        //TODO: Error on different types with same name
        
        foreach (var attribute in attributes)
        {
            var vmType = attribute.AttributeParameter;
            if (vmType.IsValueType)
            {
                _log.ViewModelIsValueType(factory, vmType);
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
        
        var constructors = viewModel.GetConstructors();
        if (constructors.Length == 1)
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
        
        services = constructor.Parameters
            .Where(Attributes.HasFromServicesAttribute)
            .Select(p => p.Type)
            .ToArray();

        var sharedStateParams = constructor.Parameters
            .Where(Attributes.HasSharedStateAttribute)
            .ToArray();

        var validState = true;
        foreach (var param in sharedStateParams)
        {
            if (param.Type.IsValueType)
            {
                _log.StateIsValueType(constructor, param);
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
            if (param.Type.GetConstructors().All(c => c.Parameters.Any()))
            {
                _log.StateHasNoParameterlessConstructor(constructor, param);
                validState = false;
                continue;
            }
        }

        sharedState = sharedStateParams.Select(p => p.Type).ToArray();

        methodParams = constructor.Parameters
            .Where(p => !Attributes.HasFromServicesAttribute(p)
                        && !Attributes.HasSharedStateAttribute(p)
                        && !p.Type.Equals(factoryType))
            .ToArray();
        
        return validState;
    }
}