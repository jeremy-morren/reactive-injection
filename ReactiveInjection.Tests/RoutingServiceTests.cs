using System.Collections.Specialized;
using ReactiveInjection.Loader;
using Shouldly;

namespace ReactiveInjection.Tests;

public class RoutingServiceTests
{
    [Fact]
    public async Task SingleRouteShouldMatch()
    {
        var vm = new object();
        var loader = Loader(nameof(SingleRouteShouldMatch), (_, _,_) => Task.FromResult(vm));
        
        var manager = new ReactiveLoaderManager(loader);
        var service = new ReactiveLoaderService([manager]);

        var result = await service.Load(nameof(SingleRouteShouldMatch), [], default);
        result.ShouldBe(vm);
    }
    
    [Fact]
    public async Task NoMatchRouteShouldCallHandler()
    {
        var manager = new ReactiveLoaderManager(NeverMatchLoader, NeverMatchLoader);
        var service = new ReactiveLoaderService([manager]);

        var result = await service.Load(nameof(NoMatchRouteShouldCallHandler), [], default);
        result.ShouldBeNull();
    }

    [Fact]
    public async Task RouteShouldSplit()
    {
        const string route = "test/1";
        var vm = new object();
        var loader = Loader(a => a.Length == 2, (_,_, _) => Task.FromResult(vm));
        
        var manager = new ReactiveLoaderManager(loader);
        
        var service = new ReactiveLoaderService([manager]);
        var result = await service.Load(route, [], default);
        result.ShouldBe(vm);
    }
    
    [Fact]
    public async Task MultipleMatchesShouldFail()
    {
        var loader = Loader(nameof(MultipleMatchesShouldFail), (_, _, _) => throw new NotImplementedException());
        
        var manager = new ReactiveLoaderManager(loader, loader);
        var service = new ReactiveLoaderService([manager]);

        var result = await service.Load(nameof(MultipleMatchesShouldFail), [], default);
        result.ShouldBeNull();
    }
    
    private class ReactiveLoaderManager : IReactiveLoaderManager
    {
        public ReactiveLoaderManager(params IReactiveViewModelLoader[] loaders)
        {
            Loaders = loaders.Append(NeverMatchLoader).ToList();
        }

        public IReadOnlyList<IReactiveViewModelLoader> Loaders { get; }
    }

    private static readonly IReactiveViewModelLoader NeverMatchLoader = 
        Loader(_ => false, (_, _, _) => throw new NotImplementedException());
    
    private static IReactiveViewModelLoader Loader(string route,
        Func<string[], NameValueCollection, CancellationToken, Task<object>> load) =>
        Loader(a => string.Join("/", a) == route, load);

    private static IReactiveViewModelLoader Loader(Func<string[], bool> matches,
        Func<string[], NameValueCollection, CancellationToken, Task<object>> load) =>
        new ReactiveViewModelLoader<ReactiveLoaderManager>(
            typeof(object), 
            string.Empty,
            [], 
            string.Empty,
            null!,
            matches,
            (_, r, q, _, ct) => load(r, q, ct));
}