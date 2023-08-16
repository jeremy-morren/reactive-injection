using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Tokens.Generator;

internal class AttributeSymbol : IAttribute
{
    public readonly AttributeData Source;

    public AttributeSymbol(Location location, AttributeData source)
    {
        Location = location;
        Source = source;
    }

    public Location Location { get; }
    
    public IType Type => new TypeSymbol(
        Source.AttributeClass ?? throw new Exception("Attribute class is null"));

    public IType AttributeParameter
    {
        get
        {
            if (Source.ConstructorArguments.Length == 0) 
                throw new Exception("Attribute has no constructor arguments");

            var arg = Source.ConstructorArguments[0];
            if (arg.Value is not ITypeSymbol value)
                throw new Exception("Constructor argument is not a type");

            return new TypeSymbol(value);
        }
    }

    public override string ToString() => Source.AttributeClass?.ToDisplayString() ?? Source.ToString();
}