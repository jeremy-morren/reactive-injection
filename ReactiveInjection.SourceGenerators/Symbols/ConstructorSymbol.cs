using Microsoft.CodeAnalysis;
using ReactiveInjection.SourceGenerators.Framework;

namespace ReactiveInjection.SourceGenerators.Symbols;

internal class ConstructorSymbol : IConstructor
{
    private readonly IMethodSymbol _source;

    public ConstructorSymbol(IMethodSymbol source)
    {
        if (source.MethodKind != MethodKind.Constructor)
            throw new ArgumentOutOfRangeException(nameof(source));
        _source = source;
    }

    public Location Location => _source.Locations.GetLocation();
    
    public IType ContainingType => new TypeSymbol(_source.ContainingType);
    public bool IsPublic => _source.DeclaredAccessibility.IsPublic();

    public IEnumerable<IAttribute> Attributes => _source.GetAttributes()
        .Where(a => a.AttributeClass != null)
        .Select(a => new AttributeSymbol(Location, a));

    public IEnumerable<IParameter> Parameters => _source.Parameters
        .Select(p => new ParameterSymbol(p));

    public override string ToString() => _source.ToDisplayString();
}