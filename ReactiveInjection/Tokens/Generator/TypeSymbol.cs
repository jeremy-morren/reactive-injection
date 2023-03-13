using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Tokens.Generator;

internal class TypeSymbol : IType
{
    private readonly ITypeSymbol _source;

    public TypeSymbol(ITypeSymbol source) => _source = source;

    public Location Location => _source.Locations.FirstOrDefault(l => l.IsInSource) ?? _source.Locations.First();

    public IAssembly Assembly => new AssemblyToken(_source.ContainingAssembly);
    
    public string? Namespace => _source.ContainingNamespace?.Name;

    public string Name => _source.Name;
    
    public string FullName => _source.MetadataName;

    public bool IsValueType => _source.IsValueType;
    public bool IsAbstract => _source.IsAbstract;

    public bool IsPartial => true; //TODO: Fix this
    
    public IType[] GetGenericArguments() => throw new NotImplementedException();

    public IMethod[] GetMethods() => throw new NotImplementedException();

    public IConstructor[] GetConstructors() => throw new NotImplementedException();
    
    public string CSharpName => throw new NotImplementedException();

    public override string ToString() => _source.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

    #region Equality

    public bool Equals(IType? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other is not TypeSymbol o) return false;
        return o._source.Equals(_source, SymbolEqualityComparer.Default);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not TypeSymbol o) return false;
        return o._source.Equals(_source, SymbolEqualityComparer.Default);
    }

    public override int GetHashCode() => SymbolEqualityComparer.Default.GetHashCode(_source);

    #endregion
}