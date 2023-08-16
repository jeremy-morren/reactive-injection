namespace ReactiveInjection.Tokens;

internal interface IAttribute : IToken
{
    public IType Type { get; }

    /// <summary>
    /// Gets the first constructor argument, which is assumed to be a type
    /// </summary>
    /// <remarks>
    /// Exists only to wrap <see cref="ReactiveFactoryAttribute"/>
    /// </remarks>
    public IType AttributeParameter { get; }
}