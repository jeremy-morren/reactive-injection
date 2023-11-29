using Microsoft.CodeAnalysis;

namespace ReactiveInjection.SourceGenerators.Symbols;

internal interface IToken
{
    public Location Location { get; }
}