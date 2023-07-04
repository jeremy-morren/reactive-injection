using System.Linq;
using System.Reflection;

namespace ReactiveInjection.Tokens.Reflection;

internal class ReflectedMethod : ReflectedTokenBase, IMethod
{
    private readonly MethodInfo _method;

    public ReflectedMethod(MethodInfo method) => _method = method;

    public string Name => _method.Name;
    public IType? ReturnType => _method.ReturnType == typeof(void) ? null : new ReflectedType(_method.ReturnType);

    public IType ContainingType => new ReflectedType(_method.DeclaringType!);
    
    public IParameter[] GetParameters() => _method.GetParameters()
        .Select((p, i) => (IParameter) new ReflectedParameter(i, p))
        .ToArray();
    
    public IAttribute[] GetCustomAttributes() => _method.GetCustomAttributes(true)
        .Select(a => (IAttribute)new ReflectedAttribute(a))
        .ToArray();

    public bool IsPartialDefinition => true; //For testing purposes, we just pretend

    public override string ToString() => Name;
}