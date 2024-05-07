using ReactiveInjection.SourceGenerators.Framework;
using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.SourceGenerators.DependencyInjection;

internal class ErrorLogWriter
{
    private readonly IErrorLog _log;

    public ErrorLogWriter(IErrorLog log) => _log = log;

    public void FatalError(IType factory, Exception e)
    {
        _log.WriteError(factory.Location,
            "RI1000",
            "Unexpected error generating ViewModel factory implementation",
            "An unexpected error occurred generating the view model factory '{0}': {1}: {2}",
            factory,
            e.GetType(),
            e.Message);
    }
    
    public void FactoryIsNotPartial(IType factory)
    {
        _log.WriteError(factory.Location,
            "RI1010",
            "ViewModel factory must be partial",
            "ViewModel factory '{0}' must be declared as partial to enable source generation",
            factory);
    }
    
    public void FactoryIsGenericType(IType factory)
    {
        _log.WriteError(factory.Location,
            "RI1011",
            "ViewModel factory cannot be generic type",
            "ViewModel factory '{0}' cannot be generic type",
            factory);
    }
    
    public void DuplicateViewModel(IType factory, IType viewModel)
    {
        _log.WriteError(factory.Location,
            "RI1012",
            "Duplicate ViewModel type",
            "ViewModel factory '{0}' has duplicate ViewModel type '{1}'",
            factory, viewModel);
    }
    
    public void ViewModelNotReferenceType(IType factory, IType viewModel)
    {
        _log.WriteError(factory.Location,
            "RI1020",
            "View model must be reference type",
            "View model factory '{0}': ViewModel '{1}' must be reference type",
            factory,
            viewModel);
    }
    
    public void ViewModelIsAbstract(IType factory, IType viewModel)
    {
        _log.WriteError(factory.Location,
            "RI1021",
            "View model cannot be abstract",
            "View model factory '{0}': ViewModel '{1}' cannot be abstract",
            factory,
            viewModel);
    }
    
    public void MultipleConstructorsDefined(IType factory, IType viewModel)
    {
        _log.WriteError(viewModel.Location,
            "RI1030",
            "Multiple constructors defined on ViewModel",
            "ViewModel '{0}' from factory '{1}' has multiple constructors defined",
            viewModel,
            factory);
    }
    
    public void StateNotReferenceType(IType viewModel, IParameter parameter)
    {
        _log.WriteError(parameter.Location,
            "RI1031",
            "Shared state must be reference type",
            "Shared state '{0}' on ViewModel '{1}': shared state must be reference type",
            parameter.Type, viewModel);
    }
    
    public void StateIsAbstractType(IType viewModel, IParameter parameter)
    {
        _log.WriteError(parameter.Location,
            "RI1032",
            "Shared state cannot be abstract type",
            "Shared state '{0}' on ViewModel '{1}': shared state cannot be abstract type",
            parameter.Type, viewModel);
    }
    
    public void StateHasNoParameterlessConstructor(IType viewModel, IParameter parameter)
    {
        _log.WriteError(parameter.Location,
            "RI1033",
            "Shared state type must have parameterless constructor defined",
            "Shared state '{0}' on ViewModel '{1}' has no parameterless constructor",
            parameter.Type, viewModel);
    }

    public void IncorrectLoaderSignature(IType viewModel, IMethod method)
    {
        _log.WriteError(method.Location,
            "RI1040",
            "Incorrect loader method signature",
            "Loader '{0}' on ViewModel '{1}' must return Task<T> where T is the ViewModel type",
            method, viewModel);
    }
}