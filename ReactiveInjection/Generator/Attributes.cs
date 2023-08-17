using ReactiveInjection.Tokens;

namespace ReactiveInjection.Generator;

internal static class Attributes
{
    private static bool IsType(IAttribute attribute, string name)
    {
        return attribute.Type.Name.Equals(name, StringComparison.Ordinal);
    }
    
    public static bool IsReactiveFactoryAttribute(IAttribute attribute)
    {
        return IsType(attribute, "ReactiveFactoryAttribute");
    }
    
    public static bool HasFromServicesAttribute(IParameter parameter)
    {
        return parameter.Attributes
            .Any(a => IsType(a, "FromServicesAttribute"));
    }
    
    public static bool HasSharedStateAttribute(IParameter parameter)
    {
        return parameter.Attributes
            .Any(a => IsType(a, "SharedStateAttribute"));
    }
}