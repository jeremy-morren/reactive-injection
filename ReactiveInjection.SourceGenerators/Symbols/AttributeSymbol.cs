using Microsoft.CodeAnalysis;

namespace ReactiveInjection.SourceGenerators.Symbols;

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

    public override string ToString() => _source.ToString();

    public IType TypeParameter => GetConstructorArg(arg =>
    {
        if (arg.Value is not ITypeSymbol source)
            throw new ArgumentException("Invalid attribute parameter");
        return new TypeSymbol(source);
    });

    public string StringParameter => GetConstructorArg(arg =>
        arg.Value as string ?? throw new ArgumentException("Invalid attribute parameter"));
    
    public string? StringParameterNullable
    {
        get
        {
            var args = _source.ConstructorArguments;
            if (args.Length == 0) return null;
            var value = args[0].Value;
            if (value == null) return null;
            return value as string ?? throw new ArgumentException("Invalid attribute parameter");
        }
    }

    private T GetConstructorArg<T>(Func<TypedConstant, T> selector)
    {
        if (_source.ConstructorArguments.Length == 0) 
            throw new Exception("Attribute has no constructor arguments");

        var arg = _source.ConstructorArguments[0];
        return selector(arg);
    }
}