using Moq;
using ReactiveInjection.Routing;
using Shouldly;

namespace ReactiveInjection.Tests;

public class RoutingTests
{
    [Fact]
    public async Task SingleRouteShouldMatch()
    {
        var vm = new object();
        var loader = Loader(nameof(SingleRouteShouldMatch), (_, _) => Task.FromResult(vm));
        
        var handler = new Mock<IReactiveRouterHandler>();
        handler.SetupGet(h => h.LoadTimeout)
            .Returns(TimeSpan.FromSeconds(30))
            .Verifiable();
        
        var manager = new ReactiveRoutingManger(handler, loader);

        var result = await manager.Load(nameof(SingleRouteShouldMatch));
        result.ShouldBe(vm);
        
        handler.Verify();
        handler.Verify(h => h.Matched(nameof(SingleRouteShouldMatch), loader), Times.Once);
        handler.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task NoMatchRouteShouldCallHandler()
    {
        var handler = new Mock<IReactiveRouterHandler>();

        var manager = new ReactiveRoutingManger(handler, NeverMatchLoader, NeverMatchLoader);

        var result = await manager.Load(nameof(NoMatchRouteShouldCallHandler));
        result.ShouldBeNull();
        
        handler.Verify();
        handler.Verify(h => h.NotFound(nameof(NoMatchRouteShouldCallHandler)));
        handler.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RouteShouldSplit()
    {
        const string route = "test/1";
        var vm = new object();
        var loader = Loader(a => a.Length == 2, (_, _) => Task.FromResult(vm));
        
        var handler = new Mock<IReactiveRouterHandler>();
        handler.SetupGet(h => h.LoadTimeout)
            .Returns(TimeSpan.FromSeconds(30))
            .Verifiable();
        
        var manager = new ReactiveRoutingManger(handler, loader);

        var result = await manager.Load(route);
        result.ShouldBe(vm);
        
        handler.Verify();
        handler.Verify(h => h.Matched(route, loader), Times.Once);
        handler.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task MultipleMatchesShouldFail()
    {
        var loader = Loader(nameof(MultipleMatchesShouldFail), (_, _) => throw new NotImplementedException());
        
        var handler = new Mock<IReactiveRouterHandler>();
        
        var manager = new ReactiveRoutingManger(handler, loader, loader);

        var result = await manager.Load(nameof(MultipleMatchesShouldFail));
        result.ShouldBeNull();
        
        handler.Verify(h => h.MultipleMatches(nameof(MultipleMatchesShouldFail),
            It.Is<IReadOnlyList<ReactiveViewModelLoader>>(list =>
                list.Count == 2 && list[0] == loader && list[1] == loader)),
            Times.Once());
        handler.VerifyNoOtherCalls();
    }
    
    private class ReactiveRoutingManger : ReactiveRoutingManagerBase
    {
        public ReactiveRoutingManger(Mock<IReactiveRouterHandler> handler, params ReactiveViewModelLoader[] loaders)
            : base(handler.Object, loaders.Concat([NeverMatchLoader]).ToArray())
        {
            
        }
    }

    private static readonly ReactiveViewModelLoader NeverMatchLoader = 
        Loader(_ => false, (_, _) => throw new NotImplementedException());

    
    private static ReactiveViewModelLoader Loader(string route,
        Func<string[], CancellationToken, Task<object>> load) =>
        Loader(a => string.Join("/", a) == route, load);
    
    private static ReactiveViewModelLoader Loader(Func<string[], bool> matches,
        Func<string[], CancellationToken, Task<object>> load) =>
        new(typeof(object), string.Empty, string.Empty, matches, load);

}