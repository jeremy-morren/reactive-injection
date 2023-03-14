using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Tokens.Generator;

internal class AttributeSymbol : IAttribute
{
    private readonly INamedTypeSymbol _attributeClass;

    public AttributeSymbol(Location location, AttributeData source)
    {
        Location = location;
        _attributeClass = source.AttributeClass ?? throw new ArgumentException("Attribute class is null");
    }

    public Location Location { get; }
    public IType Type => new TypeSymbol(_attributeClass);

    public override string ToString() => _attributeClass.ToDisplayString();
}