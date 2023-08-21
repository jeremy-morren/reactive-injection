namespace ReactiveInjection.Symbols;

internal interface IAttribute : IToken
{
    public IType Type { get; }

    /// <summary>
    /// Gets the first attribute constructor argument
    /// </summary>
    public object? Parameter { get; }
}