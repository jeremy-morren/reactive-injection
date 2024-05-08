// ReSharper disable ParameterTypeCanBeEnumerable.Global
namespace ReactiveInjection.Routing;

public interface IReactiveRouterHandler
{
    /// <summary>
    /// The timeout for loading a view model
    /// </summary>
    public TimeSpan LoadTimeout { get; }
    
    /// <summary>
    /// Invoked when a path is matched to a loader
    /// </summary>
    void Matched(string path, ReactiveViewModelLoader loader);
    
    /// <summary>
    /// Invoked after a loader has successfully loaded a view model
    /// </summary>
    void Loaded(string path, ReactiveViewModelLoader loader, TimeSpan elapsed);
    
    /// <summary>
    /// Invoked when no loader matches a path
    /// </summary>
    void NotFound(string path);
    
    /// <summary>
    /// Invoked when multiple loaders match a path
    /// </summary>
    void MultipleMatches(string path, IReadOnlyList<ReactiveViewModelLoader> loaders);

    /// <summary>
    /// Invoked when a loader throws an exception 
    /// </summary>
    void Error(string route, ReactiveViewModelLoader loader, Exception ex, TimeSpan elapsed);
}