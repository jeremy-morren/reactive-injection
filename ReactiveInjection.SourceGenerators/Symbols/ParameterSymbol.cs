using Microsoft.CodeAnalysis;

namespace ReactiveInjection.SourceGenerators.Symbols;

internal class ParameterSymbol : IParameter
{
    private readonly IParameterSymbol _source;

    public ParameterSymbol(IParameterSymbol source) => _source = source;

    public Location Location => _source.Locations.GetLocation();

    public int Ordinal => _source.Ordinal;
    
    public string Name => _source.Name;

    public IType Type => new TypeSymbol(_source.Type);

    public IEnumerable<IAttribute> Attributes => _source.GetAttributes()
        .Where(a => a.AttributeClass != null)
        .Select(a => (IAttribute) new AttributeSymbol(Location, a));

    public override string ToString() => _source.ToDisplayString();
}