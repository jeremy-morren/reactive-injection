using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Symbols;

internal class MethodSymbol : IMethod
{
    private readonly IMethodSymbol _source;
    
    public MethodSymbol(ISymbol symbol)
    {
        if (symbol is not IMethodSymbol m)
            throw new ArgumentOutOfRangeException(nameof(symbol));
        _source = m;
    }

    public Location Location => _source.Locations.GetLocation();
    public string Name => _source.Name;
    public IType? ReturnType => _source.ReturnsVoid ? null : new TypeSymbol(_source.ReturnType);

    public IEnumerable<IParameter> Parameters => _source.Parameters
        .Select(p => new ParameterSymbol(p));

    public IEnumerable<IAttribute> Attributes => _source.GetAttributes()
        .Where(a => a.AttributeClass != null)
        .Select(a => new AttributeSymbol(Location, a));

    public override string ToString() => _source.ToDisplayString();
}