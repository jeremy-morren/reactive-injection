using System.Reflection;
using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.Tests.Reflection;

internal class ReflectedConstructor(Type declaringType, ConstructorInfo source) : ReflectedTokenBase, IConstructor
{
    public IType ContainingType => new ReflectedType(declaringType);

    public IEnumerable<IAttribute> Attributes => source.GetCustomAttributes(true)
        .Select(a => new ReflectedAttribute(a));

    public IEnumerable<IParameter> Parameters => source.GetParameters()
        .Select((p, i) => new ReflectedParameter(i, p));
}