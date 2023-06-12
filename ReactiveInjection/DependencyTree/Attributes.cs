using System.Linq;
using ReactiveInjection.Tokens;

namespace ReactiveInjection.DependencyTree;

internal static class Attributes
{
    //NB: The generated .nuspec file excludes Abstractions assembly during build & analyze
    //Therefore we can't reference the types directly (rather we have to use strings here)
    
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