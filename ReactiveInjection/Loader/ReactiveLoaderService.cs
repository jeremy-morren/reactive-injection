using System.Collections.Specialized;
using System.Diagnostics;
using System.Web;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ReactiveInjection.Loader;

/// <summary>
/// Loads view models from routes
/// </summary>
[PublicAPI]
public class ReactiveLoaderService
{
    private readonly ILogger<ReactiveLoaderService> _logger;
    private readonly List<IReactiveLoaderManager> _managers;

    public ReactiveLoaderService(
        IEnumerable<IReactiveLoaderManager> managers,
        ILogger<ReactiveLoaderService>? logger = null)
    {
        _logger = logger ?? NullLogger<ReactiveLoaderService>.Instance;
        _managers = managers.ToList();
    }
    
    public IEnumerable<IReactiveViewModelLoader> Loaders => _managers.SelectMany(m => m.Loaders);

    /// <summary>
    /// Attempts to get the view model type for the given route
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Type? GetViewModelType(string path) => Match(path, LogLevel.Information)?.Loader.ViewModel;

    /// <summary>
    /// Attempts to load a view model for the given route, returning null if loading fails
    /// </summary>
    /// <param name="path">The route to load</param>
    /// <param name="parameters">Additional parameters to pass to loader</param>
    /// <param name="cancellationToken">CancellationToken to pass to loader</param>
    public async Task<LoadedViewModel?> Load(string path, object[] parameters, CancellationToken cancellationToken)
    {
        if (parameters == null) throw new ArgumentNullException(nameof(parameters));
        
        var match = Match(path, LogLevel.Debug); //Debug level, log already matched by GetViewModelType
        if (match == null) return null; //No match, error already logged
        var start = Stopwatch.GetTimestamp();
        var result = await match.Loader.Load(match.Segments, match.Query, parameters, cancellationToken);
        var elapsed = GetElapsed(start);
        _logger.LogInformation("Loaded view model. Path: {Path}. Elapsed: {Elapsed:#,0.##}ms. Type: {ViewModelType}", 
            path, elapsed.TotalMilliseconds, result.GetType());
        return new LoadedViewModel(result, path, match.Loader, elapsed);
    }
    
    private record MatchResult(string[] Segments, NameValueCollection Query, IReactiveViewModelLoader Loader);
    
    /// <summary>
    /// Matches a route to a loader
    /// </summary>
    private MatchResult? Match(string path, LogLevel level)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));

        if (!Uri.TryCreate(path, UriKind.Relative, out _))
            throw new ArgumentOutOfRangeException(nameof(path), path, "Path is not a valid relative URI");
        
        ParseRoute(path, out var segments, out var query);

        var matches = _managers
            .SelectMany(m => m.Loaders.Select(l => new MatchResult(segments, query, l)))
            .Where(x => x.Loader.MatchesRoute(segments))
            .ToList();
        
        switch (matches.Count)
        {
            case 1:
                //1 route matches, main case
                _logger.Log(level, "Matched path {Path} to loader {Loader}", path, matches[0].Loader.Method);
                return matches[0];
            case 0:
                //Not found
                _logger.LogError("No loader found for path {Path}", path);
                return null;
            default:
                //Multiple matches
                var methods = matches.Select(m => m.Loader.Method);
                _logger.LogError("{Count} loaders found for path {Path}. Loaders: {@Loaders}", 
                    matches.Count, path, methods);
                return null;
        }
    }

    private static TimeSpan GetElapsed(long start)
    {
        var stop = Stopwatch.GetTimestamp();
        var seconds = (stop - start) / (double)Stopwatch.Frequency;
        return TimeSpan.FromSeconds(seconds);
    }

    private static void ParseRoute(string route, out string[] segments, out NameValueCollection query)
    {
        if (route.EndsWith("/"))
            //Remove trailing slash
            route = route.Substring(0, route.Length - 1);
        var uri = new Uri(new Uri("http://localhost"), route);
        segments = uri.AbsolutePath.Substring(1).Split('/');
        for (var i = 0; i < segments.Length; i++)
            segments[i] = HttpUtility.UrlDecode(segments[i]);
        query = HttpUtility.ParseQueryString(uri.Query);
    }
}