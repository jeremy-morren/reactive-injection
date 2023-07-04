using ReactiveInjection.DependencyTree;
using ReactiveInjection.Tests.DependencyTreeTests.Tree;
using ReactiveInjection.Tests.DependencyTreeTests.Tree.Models;
using ReactiveInjection.Tests.DependencyTreeTests.Tree.Services;
using ReactiveInjection.Tokens;
using ReactiveInjection.Tokens.Reflection;
using Xunit.Abstractions;

namespace ReactiveInjection.Tests.DependencyTreeTests;

public class BuildDependencyTreeTests
{
    private readonly ITestOutputHelper _output;

    public BuildDependencyTreeTests(ITestOutputHelper output) => _output = output;

    [Theory]
    [InlineData(typeof(ViewModelFactory))]
    [InlineData(typeof(BuildDependencyTreeTests))]
    public void GetReflectedType(Type type)
    {
        var t = new ReflectedType(type);
        Assert.NotEmpty(t.GetMethods());
        Assert.NotEmpty(t.GetConstructors());
    }
    
    [Fact]
    public void BuildDependencyTree()
    {
        var log = new FakeErrorLog();
        
        var builder = new FactoryDependencyTreeBuilder(log);
        var built = builder.Build(new ReflectedType(typeof(ViewModelFactory)), out var tree);

        Assert.Empty(log.Errors);

        Assert.True(built);

        Assert.NotEmpty(tree.FactoryMethods);

        Assert.Single(tree.SharedState);
        Assert.All(tree.SharedState, t => Assert.True(Equals<SharedState>(t)));

        Assert.Equal(3, tree.Services.Length);
        Assert.All(tree.Services, t => 
            Assert.True(Equals<Service>(t) || Equals<IServiceProvider>(t) || Equals<List<int>>(t)));
    }

    [Fact]
    public void WriteFactoryImplementation()
    {
        var log = new FakeErrorLog();
        
        var builder = new FactoryDependencyTreeBuilder(log);
        var b = builder.Build(new ReflectedType(typeof(ViewModelFactory)), out var tree);
        Assert.Empty(log.Errors);
        Assert.True(b);

        var writer = new FactoryImplementationWriter(log);

        var csharp = writer.GenerateCSharp(tree);

        Assert.NotNull(csharp);

        Assert.Empty(log.Errors);

        _output.WriteLine(csharp);
    }

    private static bool Equals<T>(IType type) => type.Equals(new ReflectedType(typeof(T)));
}