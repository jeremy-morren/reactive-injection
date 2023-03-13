using System.Reflection;

namespace ReactiveInjection.Tokens.Reflection;

internal class ReflectedParameter : ReflectedTokenBase, IParameter
{
    private readonly ParameterInfo _param;

    public ReflectedParameter(int position, ParameterInfo param)
    {
        Position = position;
        _param = param;
    }

    public string Name => _param.Name;
    
    public int Position { get; }

    public IType ParameterType => new ReflectedType(_param.ParameterType);

    public IAttribute[] GetCustomAttributes() => _param.GetCustomAttributes()
        .Select(a => (IAttribute) new ReflectedAttribute(a))
        .ToArray();

    public override string ToString() => Name;
}