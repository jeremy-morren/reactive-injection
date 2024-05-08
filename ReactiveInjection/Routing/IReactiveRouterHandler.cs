namespace ReactiveInjection.Routing;

public interface IReactiveRouterHandler
{
    /// <summary>
    /// The timeout for loading a view model
    /// </summary>
    public TimeSpan LoadTimeout { get; }
    
    /// <summary>
    /// Handle when a loader matches a route
    /// </summary>
    void Matched(string route, ReactiveViewModelLoader loader);
    
    /// <summary>
    /// Handle when no loader matches a route
    /// </summary>
    void NotFound(string route);
    
    /// <summary>
    /// Handle when multiple loaders match a route
    /// </summary>
    void MultipleMatches(string route, IReadOnlyList<ReactiveViewModelLoader> loaders);

    /// <summary>
    /// Handle when a loader throws an exception 
    /// </summary>
    void Error(string route, ReactiveViewModelLoader loader, Exception ex);
}