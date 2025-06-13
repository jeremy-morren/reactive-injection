using System.Collections.Specialized;
using System.Reflection;
using JetBrains.Annotations;

namespace ReactiveInjection.Loader;

[PublicAPI]
public interface IReactiveViewModelLoader
{
    /// <summary>
    /// The view model type returned by <c>Load</c>
    /// </summary>
    Type ViewModel { get; }

    /// <summary>
    /// The method on <see cref="ViewModel"/> called by <c>Load</c>
    /// </summary>
    MethodInfo Method { get; }

    /// <summary>
    /// The route template that this loader matches (from <see cref="LoaderRouteAttribute"/>)
    /// </summary>
    string RouteTemplate { get; }

    /// <summary>
    /// Checks if the route matches the given path
    /// </summary>
    /// <param name="segments">Path to match (split by /)</param>
    bool MatchesRoute(string[] segments);

    /// <summary>
    /// Loads the view model for the given route
    /// </summary>
    /// <param name="route">Route to load (split by /)</param>
    /// <param name="query">Query string</param>
    /// <param name="parameters">Additional parameters for loader</param>
    /// <param name="ct">CancellationToken to pass to loader method</param>
    Task<object> Load(string[] route, NameValueCollection query, object[] parameters, CancellationToken ct);
}