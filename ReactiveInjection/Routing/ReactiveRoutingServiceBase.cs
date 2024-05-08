using System.Diagnostics;

namespace ReactiveInjection.Routing;

public abstract class ReactiveRoutingServiceBase<TService> where TService : ReactiveRoutingServiceBase<TService>
{
    private readonly IReactiveRouterHandler _handler;
    private readonly ReactiveViewModelLoader<TService>[] _loaders;

    protected ReactiveRoutingServiceBase(IReactiveRouterHandler handler, 
        ReactiveViewModelLoader<TService>[] loaders)
    {
        _handler = handler;
        _loaders = loaders;
    }

    /// <summary>
    /// Attempts to load a view model for the given route, returning null if loading fails
    /// </summary>
    /// <param name="path">The route to load</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Route is null</exception>
    public async Task<object?> Load(string path)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));

        var start = Stopwatch.GetTimestamp();

        var split = path.Split('/');
        
        var matches = _loaders.Where(l => l.MatchesRoute(split)).ToList();
        
        switch (matches.Count)
        {
            case 1:
                //1 route matches, main case
                try
                {
                    var loader = matches[0];
                    _handler.Matched(path, loader);
                    var ct = new CancellationTokenSource(_handler.LoadTimeout).Token;
                    var result = await loader.Load((TService)this, split, ct);
                    var elapsed = GetElapsed(start);
                    _handler.Loaded(path, loader, elapsed);
                    return result;
                }
                catch (Exception ex)
                {
                    var elapsed = GetElapsed(start);
                    _handler.Error(path, matches[0], ex, elapsed);
                    return null;
                }

            case 0:
                //Not found
                _handler.NotFound(path);
                return null;
            default:
                //Multiple matches
                _handler.MultipleMatches(path, matches);
                return null;
        }
    }

    private static TimeSpan GetElapsed(long start)
    {
        var stop = Stopwatch.GetTimestamp();
        var seconds = (stop - start) / (double)Stopwatch.Frequency;
        return TimeSpan.FromSeconds(seconds);
    }
}