﻿using ReactiveInjection.Tests.DependencyInjection;
using ReactiveInjection.Tests.DependencyInjection.DependencyTreeTests;
using Shouldly;
using Tree.Models;

namespace ReactiveInjection.Tests.Reflection;

public class ReflectionTests
{
    [Theory]
    [InlineData(typeof(ViewModelFactory))]
    [InlineData(typeof(BuildDependencyTreeTests))]
    public void GetReflectedType(Type type)
    {
        var t = new ReflectedType(type);
        t.Constructors.ShouldNotBeEmpty();
        t.Attributes.ShouldNotBeEmpty();
        t.Properties.ShouldNotBeEmpty();
    }
}