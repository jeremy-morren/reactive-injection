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
    
    public object? Parameter
    {
        get
        {
            //We just return the first property
            var prop = _value.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(a => a.Name != "TypeId");
            var value = prop?.GetValue(_value);
            if (value is Type t) return new ReflectedType(t);
            return value;
        }
    }

    public override string? ToString() => Type.ToString();
}