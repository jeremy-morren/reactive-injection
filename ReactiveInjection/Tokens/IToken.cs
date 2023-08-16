using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Tokens;

internal interface IToken
{
    public Location Location { get; }
}