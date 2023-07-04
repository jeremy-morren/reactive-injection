using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Tokens.Reflection;

internal abstract class ReflectedTokenBase
{
    public Location Location => Location.None;
}