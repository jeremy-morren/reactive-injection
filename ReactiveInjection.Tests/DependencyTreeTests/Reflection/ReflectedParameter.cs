using System.Linq;
using System.Reflection;

namespace ReactiveInjection.Tokens.Reflection;

internal class ReflectedParameter : ReflectedTokenBase, IParameter
{
    private readonly ParameterInfo _param;

    public ReflectedParameter(int position, ParameterInfo param)
    {
        Ordinal = position;
        _param = param;
    }

    public string Name => _param.Name;
    
    public int Ordinal { get; }

    public IType Type => new ReflectedType(_param.ParameterType);

    public IAttribute[] GetCustomAttributes() => _param.GetCustomAttributes()
        .Select(a => (IAttribute) new ReflectedAttribute(a))
        .ToArray();

    public override string ToString() => Name;
}