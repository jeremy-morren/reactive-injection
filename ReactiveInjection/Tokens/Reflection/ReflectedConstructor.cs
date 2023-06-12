using System;
using System.Linq;
using System.Reflection;

namespace ReactiveInjection.Tokens.Reflection;

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

    public IAttribute[] GetCustomAttributes() => _source.GetCustomAttributes(true)
        .Select(a => (IAttribute) new ReflectedAttribute(a))
        .ToArray();

    public IParameter[] GetParameters() => _source.GetParameters()
        .Select((p, i) => (IParameter)new ReflectedParameter(i, p))
        .ToArray();
}