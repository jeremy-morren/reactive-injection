namespace ReactiveInjection.Tokens;

internal interface IAttribute : IToken
{
    public IType Type { get; }
}