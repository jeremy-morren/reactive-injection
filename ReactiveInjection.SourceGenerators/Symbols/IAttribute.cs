namespace ReactiveInjection.SourceGenerators.Symbols;

internal interface IAttribute : IToken
{
    IType Type { get; }

    /// <summary>
    /// Gets the first attribute constructor argument as a Type
    /// </summary>
    IType TypeParameter { get; }
    
    /// <summary>
    /// Gets the first attribute constructor argument as a string
    /// </summary>
    string StringParameter { get; }
    
    /// <summary>
    /// Gets the first attribute constructor argument as a string, or null if no arguments
    /// </summary>
    string? StringParameterNullable { get; }
}