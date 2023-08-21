using System.Reflection;
using ReactiveInjection.Symbols;

namespace ReactiveInjection.Tests.Reflection;

internal class ReflectedConstructor : ReflectedTokenBase, IConstructor
{
    private readonly Type _declaringType;
    private readonly ConstructorInfo _source;

    public ReflectedConstructor(Type declaringType, ConstructorInfo source)
    {
        _declaringType = declaringType;
        _source = source;
    }

    public IType ContainingType => new ReflectedType(_declaringType);

    public IEnumerable<IAttribute> Attributes => _source.GetCustomAttributes(true)
        .Select(a => new ReflectedAttribute(a));

    public IEnumerable<IParameter> Parameters => _source.GetParameters()
        .Select((p, i) => new ReflectedParameter(i, p));
}