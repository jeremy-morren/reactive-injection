

// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace ReactiveInjection.Tokens;

internal interface IType : IToken, IEquatable<IType> 
{
    public IAssembly Assembly { get; }
    
    public string? Namespace { get; }

    public string Name { get; }

    public string FullName { get; }

    public bool IsValueType { get; }
    
    public bool IsAbstract { get; }

    public bool IsPartial { get; }

    /// <summary>
    /// Searches for all public instance methods methods defined for the current <see cref="IType" />
    /// </summary>
    /// <remarks>
    /// This is the equivalent of  of <c>Type.GetMethods(BindingFlags.Public | BindingFlags.Instance)</c>
    /// </remarks>
    public IMethod[] GetMethods();

    /// <summary>
    /// Searches for all public constructors
    /// </summary>
    /// <remarks>
    /// This is the equivalent of  of <c>Type.Constructors(BindingFlags.Public | BindingFlags.Instance)</c>
    /// </remarks>
    public IConstructor[] GetConstructors();

    /// <summary>
    /// Gets a fully qualified CSharp type name
    /// (ie fully opened generic type)
    /// </summary>
    public string CSharpName { get; }
}