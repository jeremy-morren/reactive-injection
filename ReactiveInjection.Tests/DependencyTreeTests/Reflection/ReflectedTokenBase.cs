using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Tests.DependencyTreeTests.Reflection;

internal abstract class ReflectedTokenBase
{
    public Location Location => Location.None;
}