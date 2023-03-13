using ReactiveInjection.Tokens;

namespace ReactiveInjection.DependencyTree;

internal static class Attributes
{
    public static bool HasReactiveFactoryAttribute(IMethod method)
    {
        return method.GetCustomAttributes()
            .Where(IsReactiveInjectionAbstractionsAssembly)
            .Any(a => a.Type is
            {
                FullName: "ReactiveInjection.ReactiveFactoryAttribute"
            });
    }
    
    public static bool HasFromDIAttribute(IParameter parameter)
    {
        return parameter.GetCustomAttributes()
            .Where(IsReactiveInjectionAbstractionsAssembly)
            .Any(a => a.Type is
            {
                FullName: "ReactiveInjection.FromDIAttribute"
            });
    }

    private static bool IsReactiveInjectionAbstractionsAssembly(IAttribute attribute) => 
        attribute.Type.Assembly.Name == "ReactiveInjection.Abstractions";
}