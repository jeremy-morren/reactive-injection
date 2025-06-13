

// ReSharper disable ReturnTypeCanBeEnumerable.Global

using JetBrains.Annotations;

namespace ReactiveInjection.SourceGenerators.Symbols;

[PublicAPI]
internal interface IType : IToken, IEquatable<IType>
{
    IAssembly Assembly { get; }

    /// <summary>
    /// Gets the containing type (for nested class)
    /// </summary>
    IType? ContainingType { get; }

    string? Namespace { get; }

    string Name { get; }

    string FullName { get; }

    /// <summary>
    /// Gets a fully qualified CSharp type name (ie fully opened generic type),
    /// including 'global::' prefix and nullable qualifier
    /// </summary>
    string CSharpName { get; }

    bool IsReferenceType { get; }

    bool IsAbstract { get; }

    bool IsPartial { get; }
    
    bool IsGenericType { get; }

    bool IsNullable { get; }
    
    /// <summary>
    /// Whether the type is a primitive (int, string, etc.)
    /// </summary>
    bool IsPrimitive { get; }
    
    /// <summary>
    /// Gets the underlying type of a nullable type, if the type is Nullable{T}
    /// </summary>
    /// <returns></returns>
    IType GetUnderlyingType();
    
    /// <summary>
    /// Gets type generic arguments
    /// </summary>
    IEnumerable<IType> GenericArguments { get; }

    /// <summary>
    /// Gets all attributes applied to a type
    /// </summary>
    /// <returns></returns>
    IEnumerable<IAttribute> Attributes { get; }

    /// <summary>
    /// Searches for all constructors
    /// </summary>
    /// <remarks>
    /// This is the equivalent of  of <c>Type.Constructors(BindingFlags.| BindingFlags.Instance)</c>
    /// </remarks>
    IEnumerable<IConstructor> Constructors { get; }
    
    /// <summary>
    /// Gets all type properties
    /// </summary>
    IEnumerable<IProperty> Properties { get; }
    
    /// <summary>
    /// methods
    /// </summary>
    IEnumerable<IMethod> Methods { get; }
    
    /// <summary>
    /// Gets implemented interfaces
    /// </summary>
    IEnumerable<IType> Interfaces { get; }
}