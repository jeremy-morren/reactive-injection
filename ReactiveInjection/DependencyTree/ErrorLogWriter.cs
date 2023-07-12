using ReactiveInjection.Tokens;

namespace ReactiveInjection.DependencyTree;

internal class ErrorLogWriter
{
    private readonly IErrorLog _log;

    public ErrorLogWriter(IErrorLog log) => _log = log;

    public void FactoryMethodReturnsVoid(IType factory, IMethod method)
    {
        _log.WriteError(method.Location,
            "RI1010",
            "Invalid factory method return type",
            "Factory method '{0}' on factory '{1}' has an invalid return type",
            factory,
            method);
    }
    
    public void FactoryMethodNotMarkedAsPartial(IType factory, IMethod method)
    {
        _log.WriteError(method.Location,
            "RI1011",
            "Factory method must be marked as partial",
            "Factory method '{0}' on factory '{1}' must be marked as partial",
            factory,
            method);
    }
    
    public void MultipleConstructorsDefined(IType factory, IType viewModel)
    {
        _log.WriteError(viewModel.Location,
            "RI1020",
            "Multiple constructors defined on ViewModel",
            "ViewModel '{0}' from factory '{1}' has multiple constructors defined",
            viewModel,
            factory);
    }
    
    public void StateIsValueType(IConstructor constructor, IParameter parameter)
    {
        _log.WriteError(parameter.Location,
            "RI1021",
            "Shared state cannot be value type",
            "Shared state '{0}' on ViewModel '{1}': shared state cannot be value type",
            parameter.Type,
            constructor.ContainingType);
    }
    
    public void StateIsAbstractType(IConstructor constructor, IParameter parameter)
    {
        _log.WriteError(parameter.Location,
            "RI1022",
            "Shared state cannot be abstract type",
            "Shared state '{0}' on ViewModel '{1}': shared state cannot be abstract type",
            parameter.Type,
            constructor.ContainingType);
    }
    
    public void StateHasNoParameterlessConstructor(IConstructor constructor, IParameter parameter)
    {
        _log.WriteError(parameter.Location,
            "RI1023",
            "Shared state type must have parameterless constructor defined",
            "Shared state '{0}' on ViewModel '{1}' has no parameterless constructor",
            parameter.Type,
            constructor.ContainingType);
    }
}