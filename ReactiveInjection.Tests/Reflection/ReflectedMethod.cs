using System.Reflection;
using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.Tests.Reflection;

internal class ReflectedMethod : ReflectedTokenBase, IMethod
{
    private readonly MethodInfo _method;

    public ReflectedMethod(MethodInfo method) => _method = method;

    public string Name => _method.Name;
    public IType? ReturnType => _method.ReturnType == typeof(void) ? null : new ReflectedType(_method.ReturnType);
    
    public IEnumerable<IParameter> Parameters => _method.GetParameters()
        .Select((p, i) => new ReflectedParameter(i, p));
    
    public IEnumerable<IAttribute> Attributes => _method.GetCustomAttributes(true)
        .Select(a => new ReflectedAttribute(a));

    public override string ToString() => Name;
}