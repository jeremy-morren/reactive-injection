using JetBrains.Annotations;

namespace ReactiveInjection.Routing;

/// <summary>
/// A loader for a view model
/// </summary>
[PublicAPI]
public abstract class ReactiveViewModelLoader
{
    protected ReactiveViewModelLoader(Type viewModel,
        string loaderMethod,
        string routeTemplate)
    {
        ViewModel = viewModel;
        LoaderMethod = loaderMethod;
        RouteTemplate = routeTemplate;
    }
    
    /// <summary>
    /// The view model type returned by <c>Load</c>
    /// </summary>
    public Type ViewModel { get; }
    
    /// <summary>
    /// The method on <see cref="ViewModel"/> called by <c>Load</c>
    /// </summary>
    public string LoaderMethod { get; }
    
    /// <summary>
    /// The route template that this loader matches (from <see cref="NavigationRouteAttribute"/>)
    /// </summary>
    [RouteTemplate] public string RouteTemplate { get; }
}