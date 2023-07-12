using ReactiveInjection.Tokens;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace ReactiveInjection.DependencyTree;

internal class FactoryDependencyTreeBuilder
{
    private readonly ErrorLogWriter _log;

    public FactoryDependencyTreeBuilder(IErrorLog log) => _log = new ErrorLogWriter(log);

    public bool Build(IType factory, out FactoryDependencyTree tree)
    {
        var valid = true;
        var factories = new List<(IMethod Method, IConstructor Constructor, IType[] Dependencies, IType[] SharedState)>();

        //Get methods annotated with 'ReactiveFactoryAttribute'
        foreach (var method in factory.GetMethods().Where(Attributes.HasReactiveFactoryAttribute))
        {
            if (method.ReturnType == null)
            {
                //Returns void
                _log.FactoryMethodReturnsVoid(factory, method);
                valid = false;
                continue;
            }

            if (!method.IsPartialDefinition)
            {
                //Is defined i.e. not partial
                _log.FactoryMethodNotMarkedAsPartial(factory, method);
                valid = false;
                continue;
            }

            if (!GetDiConstructor(factory, method.ReturnType, out var constructor))
            {
                valid = false;
                continue;
            }

            if (!ProcessParameters(factory, method, constructor, out var sharedState, out var dependencies))
            {
                valid = false;
                continue;
            }

            factories.Add((method, constructor, dependencies, sharedState));
        }

        if (!valid)
        {
            tree = null!;
            return false;
        }

        //Get constructors

        tree = new FactoryDependencyTree()
        {
            FactoryType = factory,
            FactoryMethods = factories
                .Select(f => new FactoryMethod()
                {
                    Method = f.Method,
                    ReturnType = f.Method.ReturnType!,
                    Constructor = f.Constructor
                })
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

    private bool GetDiConstructor(IType factory, IType viewModel, out IConstructor constructor)
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

    private bool ProcessParameters(IType factory, 
        IMethod method, 
        IConstructor constructor,
        out IType[] sharedState,
        out IType[] services)
    {
        //Shared state is:
        //All parameters not marked as [FromDI]
        //Where parameter is not provided from method

        services = constructor.GetParameters()
            .Where(Attributes.HasFromDIAttribute)
            .Select(p => p.Type)
            .ToArray();

        var methodParams = method.GetParameters()
            .Select(p => p.Name)
            .ToList();
        
        var constructorParams = constructor.GetParameters()
            .Where(p => !Attributes.HasFromDIAttribute(p));

        var validState = true;
        var state = new List<IType>();
        foreach (var param in constructorParams)
        {
            //If it is a method param, then ignore
            if (methodParams.Contains(param.Name))
                continue;
            //Special case: if it is the factory, ignore
            if (param.Type.Equals(factory))
                continue;
            //Ensure it has a parameterless constructor
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
            if (param.Type.GetConstructors().All(c => c.GetParameters().Length != 0))
            {
                _log.StateHasNoParameterlessConstructor(constructor, param);
                validState = false;
                continue;
            }
            state.Add(param.Type);
        }
        sharedState = state.ToArray();
        return validState;
    }
}