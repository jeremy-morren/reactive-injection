using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Tokens.Generator;

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

    public IParameter[] GetParameters() => _source.Parameters
        .Select(p => (IParameter) new ParameterSymbol(p))
        .ToArray();

    public IAttribute[] GetCustomAttributes() => _source.GetAttributes()
        .Where(a => a.AttributeClass != null)
        .Select(a => (IAttribute) new AttributeSymbol(Location, a))
        .ToArray();

    public bool IsPartialDefinition => _source.IsPartialDefinition;

    public override string ToString() => _source.ToDisplayString();
}