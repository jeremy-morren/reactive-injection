using Microsoft.Extensions.DependencyInjection;

namespace ReactiveInjection.Tests.Generator;

public class CreateViewModelTests
{
    [Fact]
    public void CreateParent()
    {
        using var sp = BuildServiceProvider(out var service, out var factory);

        var vm = factory.NewParent();
        Assert.NotNull(vm);
        Assert.Same(service, vm.Service);
        Assert.NotNull(vm.SharedState);
    }
    
    [Fact]
    public void CreateChild()
    {
        using var sp = BuildServiceProvider(out var service, out var factory);

        var model = new Tree.Model();

        var vm = factory.NewChild(model);
        Assert.NotNull(vm);
        Assert.Same(service, vm.Service);
        Assert.NotNull(vm.SharedState);
        Assert.Same(model, vm.Model);
    }
    
    [Fact]
    public void SharedStateShouldBeSame()
    {
        using var sp = BuildServiceProvider(out _, out var factory);

        var parent = factory.NewParent();
        var child = factory.NewChild(new Tree.Model());
        Assert.Same(parent.SharedState, child.SharedState);
    }

    private static ServiceProvider BuildServiceProvider(out Tree.Service service,
        out Tree.ViewModelFactory factory)
    {
        service = new Tree.Service();
        var sp = new ServiceCollection()
            .AddSingleton(service)
            .AddTransient<List<int>>()
            .AddTransient<Tree.ViewModelFactory>()
            .BuildServiceProvider();
        factory = sp.GetRequiredService<Tree.ViewModelFactory>();
        return sp;
    }
}