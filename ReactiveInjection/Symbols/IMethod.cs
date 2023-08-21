
// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace ReactiveInjection.Symbols;

internal interface IMethod : IToken
{
    public string Name { get; }
    
    /// <summary>
    /// Gets the method return type, or null if <c>void</c>
    /// </summary>
    public IType? ReturnType { get; }

    public IEnumerable<IParameter> Parameters { get; }

    public IEnumerable<IAttribute> Attributes { get; }
}