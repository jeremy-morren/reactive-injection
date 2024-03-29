﻿using System.Collections.ObjectModel;
using FluentAssertions;
using ReactiveInjection.SourceGenerators.DependencyInjection;
using ReactiveInjection.SourceGenerators.Symbols;
using ReactiveInjection.Tests.Reflection;
using Shouldly;
using Tree.Models;
using Xunit.Abstractions;

namespace ReactiveInjection.Tests.DependencyInjection;

public class BuildDependencyTreeTests(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public void BuildDependencyTree()
    {
        var log = new FakeErrorLog();

        var builder = new FactoryDependencyTreeBuilder(log);
        var built = builder.Build(new ReflectedType(typeof(ViewModelFactory), true, false), out var tree);

        log.Errors.ShouldBeEmpty();
        built.ShouldBeTrue();

        tree.ViewModels.ShouldNotBeEmpty();
        tree.ViewModels.ShouldAllBe(t => t.Type.Name.StartsWith("ViewModel"));
        tree.ViewModels.ShouldContain(t => t.MethodParams.Length > 0);
        tree.ViewModels.ShouldContain(t => t.MethodParams.Length == 0);
        
        tree.ViewModels.SelectMany(vm => vm.MethodParams)
            .ShouldNotContain(p => Equals<ViewModelFactory>(p.Type));

        tree.SharedState.Should().HaveCount(3);
        tree.SharedState.Should().Contain(new[] { typeof(SharedState), typeof(ObservableCollection<SharedState>), typeof(object) });
        
        tree.Services.Should().HaveCount(4);
        tree.Services.Should().Contain(new[] 
        { 
            typeof(Service), 
            typeof(IServiceProvider), 
            typeof(List<int>) ,
            typeof(List<int[]>) 
        });
    }

    // [Fact]
    // public void WriteFactoryImplementation()
    // {
    //     var log = new FakeErrorLog();
    //
    //     var builder = new FactoryDependencyTreeBuilder(log);
    //     var built = builder.Build(new ReflectedType(typeof(ViewModelFactory), true), out var tree);
    //     log.Errors.ShouldBeEmpty();
    //     built.ShouldBeTrue();
    //
    //     var csharp = FactoryImplementationWriter.GenerateCSharp(tree);
    //
    //     csharp.ShouldNotBeNullOrEmpty();
    //
    //     _output.WriteLine(csharp);
    // }

    private static bool Equals<T>(IType other) => new ReflectedType(typeof(T)).Equals(other);
}