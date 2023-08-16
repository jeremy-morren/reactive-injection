using System.Reflection;
using System.Runtime.InteropServices;
using ReactiveInjection.Tokens;

namespace ReactiveInjection.Tests.DependencyTreeTests.Reflection;

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
    
    public IType AttributeParameter
    {
        get
        {
            //We just return the first property
            var prop = _value.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Single(a => a.Name != "TypeId");
            var type = (Type)prop.GetValue(_value)!;
            return new ReflectedType(type);
        }
    }

    public override string? ToString() => Type.ToString();
}