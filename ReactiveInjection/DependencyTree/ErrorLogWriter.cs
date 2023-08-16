using ReactiveInjection.Tokens;

namespace ReactiveInjection.DependencyTree;

internal class ErrorLogWriter
{
    private readonly IErrorLog _log;

    public ErrorLogWriter(IErrorLog log) => _log = log;
    
    public void FactoryIsNotPartial(IType factory)
    {
        _log.WriteError(factory.Location,
            "RI1001",
            "ViewModel factory must be partial",
            "ViewModel factory '{0}' must be declared as partial to enable source generation",
            factory.FullName);
    }
    
    public void ViewModelIsValueType(IType factory, IType viewModel)
    {
        _log.WriteError(factory.Location,
            "RI1010",
            "View model cannot be value type",
            "View model factory '{0}': ViewModel '{1}' cannot be value type",
            factory.FullName,
            viewModel.FullName);
    }
    
    public void ViewModelIsAbstract(IType factory, IType viewModel)
    {
        _log.WriteError(factory.Location,
            "RI1011",
            "View model cannot be abstract",
            "View model factory '{0}': ViewModel '{1}' cannot be abstract",
            factory.FullName,
            viewModel.FullName);
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