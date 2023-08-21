using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Tests.Reflection;

internal abstract class ReflectedTokenBase
{
    public Location Location => Location.None;
}