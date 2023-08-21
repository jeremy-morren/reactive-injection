using ReactiveInjection.DependencyInjection;
using ReactiveInjection.Framework;
using ReactiveInjection.Symbols;

namespace ReactiveInjection.ModelInjection;

internal class ErrorLogWriter
{
    private readonly IErrorLog _log;

    public ErrorLogWriter(IErrorLog log) => _log = log;
    
    public void FatalError(IType viewModel, Exception e)
    {
        _log.WriteError(viewModel.Location,
            "RI1001",
            "Unexpected error generating backing backing models for ViewModel",
            "Unexpected error generating backing backing models for ViewModel '{0}': {1} {2}",
            viewModel,
            e.GetType(),
            e.Message);
    }
    
    public void ViewModelIsNotPartial(IType viewModel)
    {
        _log.WriteError(viewModel.Location,
            "RI1040",
            "ViewModel with injected model must be partial",
            "ViewModel '{0}' with injected model is not partial",
            viewModel);
    }
    
    public void PropertyNotReadable(IType viewModel, IProperty property)
    {
        _log.WriteError(property.Location,
            "RI1050",
            "Injected Model property on ViewModel is not readable",
            "Injected Model property '{0}' on ViewModel '{1}' is not readable",
            viewModel);
    }
    
    public void PropertyIsStatic(IType viewModel, IProperty property)
    {
        _log.WriteError(property.Location,
            "RI1051",
            "Injected Model property on ViewModel is static",
            "Injected Model property '{0}' on ViewModel '{1}' is static",
            viewModel);
    }
    
    public void ModelNotReferenceType(IType viewModel, IProperty property)
    {
        _log.WriteError(property.Location,
            "RI1052",
            "Injected model must be reference type",
            "Injected Model property '{0}' on ViewModel '{1}' must be reference type",
            viewModel);
    }
}