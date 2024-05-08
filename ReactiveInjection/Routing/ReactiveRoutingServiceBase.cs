﻿namespace ReactiveInjection.Routing;

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
                    return await loader.Load((TService)this, split, ct);
                }
                catch (Exception e)
                {
                    _handler.Error(path, matches[0], e);
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
}