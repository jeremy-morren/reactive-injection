using System.Reflection;
using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.Tests.Reflection;

internal class ReflectedMethod(MethodInfo method) : ReflectedTokenBase, IMethod
{
    public string Name => method.Name;
    public IType? ReturnType => method.ReturnType == typeof(void) ? null : new ReflectedType(method.ReturnType);
    
    public IEnumerable<IParameter> Parameters => method.GetParameters()
        .Select((p, i) => new ReflectedParameter(i, p));
    
    public IEnumerable<IAttribute> Attributes => method.GetCustomAttributes(true)
        .Select(a => new ReflectedAttribute(a));

    public override string ToString() => Name;
}