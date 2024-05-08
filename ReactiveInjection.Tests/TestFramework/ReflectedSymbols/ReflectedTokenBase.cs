using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Tests.TestFramework.ReflectedSymbols;

internal abstract class ReflectedTokenBase
{
    public Location Location => Location.None;
}