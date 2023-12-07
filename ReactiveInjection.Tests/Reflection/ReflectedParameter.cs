using System.Reflection;
using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.Tests.Reflection;

internal class ReflectedParameter(int position, ParameterInfo param) : ReflectedTokenBase, IParameter
{
    public string Name => param.Name!;
    
    public int Ordinal { get; } = position;

    public IType Type => new ReflectedType(param.ParameterType);

    public IEnumerable<IAttribute> Attributes => param.GetCustomAttributes()
        .Select(a => (IAttribute) new ReflectedAttribute(a))
        .ToArray();

    public override string ToString() => Name;
}