using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Symbols;

internal class AttributeSymbol : IAttribute
{
    private readonly AttributeData _source;

    public AttributeSymbol(Location location, AttributeData source)
    {
        Location = location;
        
        _source = source;
    }

    public Location Location { get; }
    
    public IType Type => new TypeSymbol(
        _source.AttributeClass ?? throw new Exception("Attribute class is null"));

    public object? Parameter
    {
        get
        {
            if (_source.ConstructorArguments.Length == 0) 
                throw new Exception("Attribute has no constructor arguments");

            var arg = _source.ConstructorArguments[0];
            if (arg.Value is ITypeSymbol type)
                return new TypeSymbol(type);
            return arg.Value;
        }
    }

    public override string ToString() => _source.AttributeClass?.ToDisplayString() ?? _source.ToString();
}