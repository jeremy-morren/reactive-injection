using Microsoft.CodeAnalysis;
using ReactiveInjection.SourceGenerators.Framework;
using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.SourceGenerators.DependencyInjection;

internal class ErrorLogWriter
{
    private readonly IErrorLog _log;

    public ErrorLogWriter(IErrorLog log) => _log = log;

    public void FatalFactoryGenerationError(IType factory, Exception e)
    {
        _log.WriteError(factory.Location,
            "RI1000",
            "Unexpected error generating ViewModel factory implementation",
            "An unexpected error occurred generating the view model factory '{0}': {1}: {2}",
            factory, e.GetType(), e.Message);
    }
    
    public void FatalRoutingGenerationError(Exception e)
    {
        _log.WriteError(Location.None,
            "RI1001",
            "Unexpected error generating reactive routing manager",
            "An unexpected error occurred generating the reactive routing manager {1}: {2}",
            e.GetType(), e.Message);
    }
    
    #region Factory
    
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
    
    #endregion
    
    #region Loader
    
    public void IncorrectLoaderSignature(IMethod method)
    {
        _log.WriteError(method.Location,
            "RI1040",
            "Incorrect loader method signature",
            "Loader '{0}' must return Task<T> where T is the ViewModel type",
            method);
    }
    
    #endregion
    
    #region Routing

    public void LoaderMustBeStatic(IMethod method)
    {
        _log.WriteError(method.Location,
            "RI1050",
            "Loader must be static",
            "View model loader {0} must be declared as static",
            method);
    }
    
    public void IncorrectRouteHandlerSignature(IMethod method)
    {
        _log.WriteError(method.Location,
            "RI1051",
            "Incorrect route handler method signature",
            "Route handler '{0}' must return T or Task<T> where T is the ViewModel type",
            method);
    }
    
    public void DuplicateRouteParameters(IAttribute attribute, string parameter)
    {
        _log.WriteError(attribute.Location,
            "RI1052",
            "Duplicate route parameter",
            "Parameter '{0}' is defined multiple times in route attribute",
            parameter);
    }
    
    public void OptionalRouteParameterIsNotLastSegment(IAttribute attribute, string parameter)
    {
        _log.WriteError(attribute.Location,
            "RI1053",
            "Optional parameter is not last segment",
            "Optional route parameter '{0}' must be the last segment in the route",
            parameter);
    }
    
    public void UnknownRouteHandlerParameterType(IParameter parameter)
    {
        _log.WriteError(parameter.Location,
            "RI1054",
            "Unknown route handler parameter type",
            "Route parameter '{0}' has an unknown type",
            parameter);
    }
    
    public void OptionalRouteParameterNotNullable(IParameter parameter)
    {
        _log.WriteError(parameter.Location,
            "RI1055",
            "Optional route parameter must be nullable",
            "Route parameter '{0}' is optional but not nullable",
            parameter);
    }
    
    public void RequiredRouteParameterNullable(IParameter parameter)
    {
        _log.WriteError(parameter.Location,
            "RI1056",
            "Required route parameter cannot be nullable",
            "Parameter '{0}' is required but has nullable type",
            parameter);
    }
    
    public void RouteHandlerParameterNotProvided(IParameter parameter)
    {
        _log.WriteError(parameter.Location,
            "RI1057",
            "Unknown route handler parameter",
            "Parameter '{0}' is not provided by the route",
            parameter);
    }
    
    public void RouteSegmentParameterNotFoundOnMethod(IMethod method, string parameter)
    {
        _log.WriteError(method.Location,
            "RI1058",
            "Route segment parameter not found on method",
            "Parameter {0} not found on method {1}",
            parameter, method);
    }
    
    #endregion
}