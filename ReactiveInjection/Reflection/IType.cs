namespace ReactiveInjection.Reflection;

public interface IType
{
    public string Namespace { get;}
    
    public string Name { get; }
    
    /// <summary>
    /// Gets the containing type, or <see langword="null"/> if
    /// type is not a nested type
    /// </summary>
    public IType? ContainingType { get; }
    
    public bool IsValueType { get; }
    
    public bool IsAbstract { get; }
    
    /// <summary>
    /// Indicates whether the type is a nullable reference type
    /// </summary>
    public bool IsNullableReferenceType { get; }

    public IMethod[] Methods { get; }

}