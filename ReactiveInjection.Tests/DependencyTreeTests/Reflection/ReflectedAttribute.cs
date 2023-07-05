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

    public override string? ToString() => Type.ToString();
}