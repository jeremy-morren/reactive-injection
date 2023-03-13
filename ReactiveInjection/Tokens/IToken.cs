using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Tokens;

internal interface IToken
{
    internal Location Location { get; }
}