namespace ReactiveInjection.SourceGenerators.Symbols;

internal interface IAttribute : IToken
{
    public IType Type { get; }

    /// <summary>
    /// Gets the first attribute constructor argument as a Type
    /// </summary>
    public IType TypeParameter { get; }
    
    /// <summary>
    /// Gets the first attribute constructor argument as a string
    /// </summary>
    public string StringParameter { get; }
}