using Microsoft.CodeAnalysis;
using ReactiveInjection.Tokens;

namespace ReactiveInjection.DependencyTree;

internal class ErrorLogWriter
{
    private readonly IErrorLog _log;

    public ErrorLogWriter(IErrorLog log) => _log = log;

    public void FactoryMethodReturnsVoid(IType factory, IMethod method)
    {
        _log.WriteError("RI1000",
            "Invalid factory method return type",
            $"Factory method {method.Name} on factory {factory} has an invalid return type",
            method.Location);
    }
    
    public void FactoryMethodNotMarkedAsPartial(IType factory, IMethod method)
    {
        _log.WriteError("RI1001",
            "Factory method must be marked as partial",
            $"Factory method {method.Name} on factory {factory} must be marked as partial",
            method.Location);
    }
    
    public void MultipleConstructorsDefined(IType factory, IType viewModel)
    {
        _log.WriteError("RI1010",
            "Multiple constructors defined on ViewModel",
            $"ViewModel {viewModel} from factory {factory} has multiple constructors defined",
            viewModel.Location);
    }
    
    public void StateIsValueType(IConstructor constructor, IParameter parameter)
    {
        _log.WriteError("RI1011",
            "Shared state cannot be value type",
            $"State {parameter.ParameterType} on ViewModel {constructor.DeclaringType}: shared state cannot be value type",
            parameter.Location);
    }
    
    public void StateIsAbstractType(IConstructor constructor, IParameter parameter)
    {
        _log.WriteError("RI1012",
            "Shared state cannot be abstract type",
            $"State {parameter.ParameterType} on ViewModel {constructor.DeclaringType}: shared state cannot be abstract type",
            parameter.Location);
    }
    
    public void StateHasNoParameterlessConstructor(IConstructor constructor, IParameter parameter)
    {
        _log.WriteError("RI1013", 
            $"Shared state type must have parameterless constructor defined", 
            $"State {parameter.ParameterType} on {constructor.DeclaringType} has no parameterless constructor",
            parameter.Location);
    }
}