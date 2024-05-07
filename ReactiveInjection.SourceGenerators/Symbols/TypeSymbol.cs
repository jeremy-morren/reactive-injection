using Microsoft.CodeAnalysis;

namespace ReactiveInjection.SourceGenerators.Symbols;

internal class TypeSymbol : IType
{
    private readonly ITypeSymbol _source;
    private readonly bool? _isPartial;

    public TypeSymbol(ITypeSymbol source, bool? isPartial = null)
    {
        _source = source;
        _isPartial = isPartial;
    }

    public Location Location => _source.Locations.GetLocation();

    public IAssembly Assembly => new AssemblySymbol(_source.ContainingAssembly);

    public IType? ContainingType => _source.ContainingType != null ? new TypeSymbol(_source.ContainingType) : null;

    public string? Namespace => _source.ContainingNamespace?.ToString();

    public string Name => _source.Name;

    public string FullName => _source.ToDisplayString();

    private NullableFlowState Nullable => _source.NullableAnnotation switch
    {
        NullableAnnotation.Annotated => NullableFlowState.MaybeNull,
        NullableAnnotation.NotAnnotated => NullableFlowState.NotNull,
        NullableAnnotation.None => NullableFlowState.None,
        _ => throw new ArgumentOutOfRangeException()
    };
    
    public string CSharpName => _source.ToDisplayString(Nullable, SymbolDisplayFormat.FullyQualifiedFormat);

    public bool IsReferenceType => _source.IsReferenceType;
    
    public bool IsAbstract => _source.IsAbstract;

    public bool IsPartial => _isPartial ?? throw new NotImplementedException();

    public bool IsGenericType => _source is INamedTypeSymbol { IsGenericType: true };
    
    public bool IsNullable => _source.NullableAnnotation == NullableAnnotation.Annotated;
    public IEnumerable<IType> GenericArguments => _source is INamedTypeSymbol { IsGenericType: true} s
        ? s.TypeArguments.Select(t => new TypeSymbol(t))
        : Enumerable.Empty<IType>();

    public IEnumerable<IAttribute> Attributes => _source.GetAttributes()
        .Select(a => new AttributeSymbol(Location, a));

    public IEnumerable<IConstructor> Constructors => _source.GetMembers()
        .OfType<IMethodSymbol>()
        .Where(s => s.MethodKind == MethodKind.Constructor)
        .Select(m => new ConstructorSymbol(m));

    public IEnumerable<IProperty> Properties => 
        _source.GetMembers().OfType<IPropertySymbol>().Select(p => new PropertySymbol(p));

    public IEnumerable<IMethod> Methods =>
        _source.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.MethodKind == MethodKind.Ordinary)
            .Select(s => new MethodSymbol(s));

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