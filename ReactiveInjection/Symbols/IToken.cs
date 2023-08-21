using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Symbols;

internal interface IToken
{
    public Location Location { get; }
}