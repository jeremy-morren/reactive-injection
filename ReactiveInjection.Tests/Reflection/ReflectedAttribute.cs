using System.Reflection;
using ReactiveInjection.Symbols;

namespace ReactiveInjection.Tests.Reflection;

internal class ReflectedAttribute : ReflectedTokenBase, IAttribute
{
    private readonly Attribute _value;

    public ReflectedAttribute(object value)
    {
        if (value is not Attribute a)
            throw new ArgumentException("Value is not an Attribute", nameof(value));
        _value = a;
    }

    public IType Type => new ReflectedType(_value.GetType());

    public IType TypeParameter => new ReflectedType((Type)GetProperty());

    public string[] StringParams => (string[])GetProperty();

    private object GetProperty()
    {
        //We just return the first property
        var prop = _value.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Single(a => a.Name != "TypeId");
        return prop.GetValue(_value)!;
    }

    public override string? ToString() => Type.ToString();
}