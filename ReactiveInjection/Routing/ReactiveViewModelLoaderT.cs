using JetBrains.Annotations;

namespace ReactiveInjection.Routing;

/// <summary>
/// A loader for a view model
/// </summary>
[PublicAPI]
public class ReactiveViewModelLoader<TManager> : ReactiveViewModelLoader
{
    private readonly Func<string[], bool> _matches;
    private readonly Func<TManager, string[], CancellationToken, Task<object>> _load;

    public ReactiveViewModelLoader(Type viewModel,
        string loaderMethod,
        [RouteTemplate] string routeTemplate,
        Func<string[], bool> matches,
        Func<TManager, string[], CancellationToken, Task<object>> load)
        : base(viewModel, loaderMethod, routeTemplate)
    {
        _matches = matches;
        _load = load;
    }
    
    /// <summary>
    /// Checks if the route matches the given path
    /// </summary>
    /// <param name="route">Route to load (split by /)</param>
    public bool MatchesRoute(string[] route) => _matches(route);

    /// <summary>
    /// Loads the view model for the given route
    /// </summary>
    /// <param name="manager">Routing manager</param>
    /// <param name="route">Route to load (split by /)</param>
    /// <param name="ct">CancellationToken to pass to loader method</param>
    public Task<object> Load(TManager manager, string[] route, CancellationToken ct) => _load(manager, route, ct);
}