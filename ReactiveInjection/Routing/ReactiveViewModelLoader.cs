using JetBrains.Annotations;

namespace ReactiveInjection.Routing;

/// <summary>
/// A loader for a view model
/// </summary>
[PublicAPI]
public class ReactiveViewModelLoader
{
    private readonly Func<string[], bool> _matches;
    private readonly Func<string[], CancellationToken, Task<object>> _load;

    public ReactiveViewModelLoader(Type viewModel,
        string loaderMethod,
        string route,
        Func<string[], bool> matches,
        Func<string[], CancellationToken, Task<object>> load)
    {
        ViewModel = viewModel;
        LoaderMethod = loaderMethod;
        Route = route;
        
        _matches = matches;
        _load = load;
    }
    
    /// <summary>
    /// The view model type returned by <see cref="Load"/>
    /// </summary>
    public Type ViewModel { get; }
    
    /// <summary>
    /// The method on <see cref="ViewModel"/> called by <see cref="Load"/>
    /// </summary>
    public string LoaderMethod { get; }
    
    /// <summary>
    /// The route that this loader matches (from <see cref="NavigationRouteAttribute"/>)
    /// </summary>
    [RouteTemplate] public string Route { get; }

    /// <summary>
    /// Checks if the route matches the given path
    /// </summary>
    /// <param name="route">Route to load (split by /)</param>
    public bool MatchesRoute(string[] route) => _matches(route);
    
    /// <summary>
    /// Loads the view model for the given route
    /// </summary>
    /// <param name="route">Route to load (split by /)</param>
    /// <param name="ct">CancellationToken to pass to loader method</param>
    public Task<object> Load(string[] route, CancellationToken ct) => _load(route, ct);
}