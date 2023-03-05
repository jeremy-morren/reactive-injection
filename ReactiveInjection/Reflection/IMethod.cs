using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Reflection;

public interface IMethod
{
    public string Name { get; }
    public IType? ReturnType { get; }

    public IAttribute[] Attributes { get; }
    
    public IParameter[] Parameters { get; }
}