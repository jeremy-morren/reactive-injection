

// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace ReactiveInjection.SourceGenerators.Symbols;

internal interface IType : IToken, IEquatable<IType> 
{
    public IAssembly Assembly { get; }

    /// <summary>
    /// Gets the containing type (for nested class)
    /// </summary>
    public IType? ContainingType { get; }

    public string? Namespace { get; }

    public string Name { get; }

    public string FullName { get; }

    /// <summary>
    /// Gets a fully qualified CSharp type name (ie fully opened generic type),
    /// including 'global::' prefix and nullable qualifier
    /// </summary>
    public string CSharpName { get; }

    public bool IsReferenceType { get; }

    public bool IsAbstract { get; }

    public bool IsPartial { get; }
    
    public bool IsGenericType { get; }

    public bool IsNullable { get; }

    /// <summary>
    /// Gets all attributes applied to a type
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IAttribute> Attributes { get; }

    /// <summary>
    /// Searches for all public constructors
    /// </summary>
    /// <remarks>
    /// This is the equivalent of  of <c>Type.Constructors(BindingFlags.Public | BindingFlags.Instance)</c>
    /// </remarks>
    public IEnumerable<IConstructor> Constructors { get; }
    
    public IEnumerable<IProperty> Properties { get; }
}