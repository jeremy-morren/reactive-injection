namespace ReactiveInjection.Routing;

public abstract class ReactiveRoutingManagerBase
{
    private readonly IReactiveRouterHandler _handler;
    private readonly ReactiveViewModelLoader[] _loaders;

    protected ReactiveRoutingManagerBase(IReactiveRouterHandler handler, 
        ReactiveViewModelLoader[] loaders)
    {
        _handler = handler;
        _loaders = loaders;
    }

    /// <summary>
    /// Attempts to load a view model for the given route, returning null if loading fails
    /// </summary>
    /// <param name="route">The route to load</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Route is null</exception>
    public async Task<object?> Load(string route)
    {
        if (route == null) throw new ArgumentNullException(nameof(route));

        var split = route.Split('/');
        
        var matches = _loaders.Where(l => l.MatchesRoute(split)).ToList();
        
        switch (matches.Count)
        {
            case 1:
                //1 route matches, main case
                try
                {
                    var loader = matches[0];
                    _handler.Matched(route, loader);
                    var ct = new CancellationTokenSource(_handler.LoadTimeout).Token;
                    return await loader.Load(split, ct);
                }
                catch (Exception e)
                {
                    _handler.Error(route, matches[0], e);
                    return null;
                }

            case 0:
                //Not found
                _handler.NotFound(route);
                return null;
            default:
                //Multiple matches
                _handler.MultipleMatches(route, matches);
                return null;
        }
    }
}