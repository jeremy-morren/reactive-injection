namespace ReactiveInjection.Reflection;

public interface IParameter
{
    public string Name { get; }

    public IType ParameterType { get; }

    public IAttribute[] Attributes { get; }
}