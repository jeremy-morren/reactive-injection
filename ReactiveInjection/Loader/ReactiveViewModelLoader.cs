using System.Collections.Specialized;
using System.Reflection;
using JetBrains.Annotations;

namespace ReactiveInjection.Loader;

/// <summary>
/// A loader for a view model
/// </summary>
[PublicAPI]
public class ReactiveViewModelLoader<TManager> : IReactiveViewModelLoader
{
    private readonly TManager _manager;
    private readonly Func<string[], bool> _matches;
    private readonly Func<TManager, string[], NameValueCollection, object[], CancellationToken, Task<object>> _load;
    
    private readonly string _methodName;
    private readonly Type[] _methodParameters;

    public ReactiveViewModelLoader(
        Type viewModel,
        string methodName,
        Type[] methodParameters,
        string routeTemplate,
        TManager manager,
        Func<string[], bool> matches,
        Func<TManager, string[], NameValueCollection, object[], CancellationToken, Task<object>> load)
    {
        ViewModel = viewModel;
        _methodName = methodName;
        _methodParameters = methodParameters;
        RouteTemplate = routeTemplate;
        
        _manager = manager;
        _matches = matches;
        _load = load;
    }

    /// <summary>
    /// The view model type returned by <c>Load</c>
    /// </summary>
    public Type ViewModel { get; }

    /// <summary>
    /// The method on <see cref="ViewModel"/> called by <c>Load</c>
    /// </summary>
    public MethodInfo Method =>
        ViewModel.GetMethod(_methodName, _methodParameters)
        ?? throw new InvalidOperationException($"Loader method not found");

    /// <summary>
    /// The route template that this loader matches (from <see cref="LoaderRouteAttribute"/>)
    /// </summary>
    [RouteTemplate] public string RouteTemplate { get; }
    
    /// <summary>
    /// Checks if the route matches the given path
    /// </summary>
    /// <param name="segments">Path to match (split by /)</param>
    public bool MatchesRoute(string[] segments) => _matches(segments);

    /// <summary>
    /// Loads the view model for the given route
    /// </summary>
    /// <param name="route">Route to load (split by /)</param>
    /// <param name="query">Query string</param>
    /// <param name="parameters"></param>
    /// <param name="ct">CancellationToken to pass to loader method</param>
    public Task<object> Load(string[] route, NameValueCollection query, object[] parameters, CancellationToken ct) => 
        _load(_manager, route, query, parameters, ct);
    
    public override string ToString() => $"{ViewModel.Name}/{_methodName}";
}