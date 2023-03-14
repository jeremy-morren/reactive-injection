using System.Reflection;
// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace ReactiveInjection.Tokens;

internal interface IMethod : IToken
{
    public string Name { get; }
    
    /// <summary>
    /// Gets the method return type, or null if <c>void</c>
    /// </summary>
    public IType? ReturnType { get; }

    public IParameter[] GetParameters();

    public IAttribute[] GetCustomAttributes();

    /// <summary>
    /// Indicates whether the method is defined
    /// (i.e. whether the 'partial' keyword is used)
    /// </summary>
    public bool IsPartialDefinition { get; }
}