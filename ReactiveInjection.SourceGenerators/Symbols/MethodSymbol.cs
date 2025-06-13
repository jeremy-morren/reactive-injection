using Microsoft.CodeAnalysis;
using ReactiveInjection.SourceGenerators.Framework;

namespace ReactiveInjection.SourceGenerators.Symbols;

internal class MethodSymbol : IMethod
{
    private readonly IMethodSymbol _source;
    
    public MethodSymbol(IMethodSymbol source)
    {
        // if (source.MethodKind != MethodKind.Ordinary)
        //     throw new ArgumentException($"Method {source.MethodKind} is not supported");
        _source = source;
    }

    public Location Location => _source.Locations.GetLocation();
    public IType ContainingType => new TypeSymbol(_source.ContainingType);
    public string Name => _source.Name;
    public bool IsStatic => _source.IsStatic;
    public bool IsPublic => _source.DeclaredAccessibility.IsPublic();
    
    public IType? ReturnType => _source.ReturnsVoid ? null : new TypeSymbol(_source.ReturnType);

    public IEnumerable<IParameter> Parameters => _source.Parameters
        .Select(p => new ParameterSymbol(p));

    public IEnumerable<IAttribute> Attributes => _source.GetAttributes()
        .Where(a => a.AttributeClass != null)
        .Select(a => new AttributeSymbol(Location, a));

    public override string ToString() => _source.ToDisplayString();
}