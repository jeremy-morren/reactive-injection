
// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace ReactiveInjection.SourceGenerators.Symbols;

internal interface IMethod : IToken
{
    public IType ContainingType { get; }
    
    public string Name { get; }
    
    public bool IsStatic { get; }
    
    /// <summary>
    /// Whether the method is a public method
    /// </summary>
    public bool IsPublic { get; }
    
    /// <summary>
    /// Gets the method return type, or null if <c>void</c>
    /// </summary>
    public IType? ReturnType { get; }

    public IEnumerable<IParameter> Parameters { get; }

    public IEnumerable<IAttribute> Attributes { get; }
}