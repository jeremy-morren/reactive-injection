using Models.ModelItems;
using ReactiveInjection.DependencyInjection;
using ReactiveInjection.ModelInjection;
using ReactiveInjection.Tests.Reflection;
using Shouldly;
using Xunit.Abstractions;

namespace ReactiveInjection.Tests.ModelInjection;

public class ModelInjectionTreeTests
{
    private readonly ITestOutputHelper _output;

    public ModelInjectionTreeTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [InlineData(typeof(InjectedViewModel1))]
    [InlineData(typeof(InjectedViewModel2))]
    [InlineData(typeof(InjectedViewModel3))]
    public void BuildDependencyTree(Type viewModel)
    {
        var log = new FakeErrorLog();

        var builder = new ModelInjectionTreeBuilder(log);

        var built = builder.Build(new ReflectedType(viewModel, true), out var tree);

        log.Errors.ShouldBeEmpty();
        built.ShouldBeTrue();

        tree.ShouldNotBeNull();

        tree.Models.ShouldNotBeEmpty();
    }

    [Theory]
    [InlineData(typeof(InjectedViewModel1))]
    [InlineData(typeof(InjectedViewModel2))]
    [InlineData(typeof(InjectedViewModel3))]
    public void Generate(Type viewModel)
    {
        var log = new FakeErrorLog();

        var builder = new ModelInjectionTreeBuilder(log);

        var built = builder.Build(new ReflectedType(viewModel, true), out var tree);

        log.Errors.ShouldBeEmpty();
        built.ShouldBeTrue();

        var csharp = InjectionImplementationWriter.Generate(tree);

        csharp.ShouldNotBeNullOrWhiteSpace();

        _output.WriteLine(csharp);
    }
}