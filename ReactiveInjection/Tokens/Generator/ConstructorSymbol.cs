using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using ReactiveInjection.Tokens.Reflection;

namespace ReactiveInjection.Tokens.Generator;

internal class ConstructorSymbol : IConstructor
{
    private readonly IMethodSymbol _source;

    public ConstructorSymbol(ISymbol source)
    {
        if (source is not IMethodSymbol { MethodKind: MethodKind.Constructor } m)
            throw new ArgumentOutOfRangeException(nameof(source));
        _source = m;
    }

    public Location Location => _source.Locations.GetLocation();
    
    public IType ContainingType => new TypeSymbol(_source.ContainingType);

    public IAttribute[] GetCustomAttributes() => _source.GetAttributes()
        .Where(a => a.AttributeClass != null)
        .Select(a => (IAttribute) new AttributeSymbol(Location, a))
        .ToArray();

    public IParameter[] GetParameters() => _source.Parameters
        .Select(p => (IParameter)new ParameterSymbol(p))
        .ToArray();

    public override string ToString() => _source.ToDisplayString();
}