using ReactiveInjection.Symbols;

namespace ReactiveInjection.Framework;

internal static class AttributeHelpers
{
    private static bool IsType(IAttribute attribute, string name)
    {
        return attribute.Type.Name.Equals(name, StringComparison.Ordinal);
    }
    
    public static bool IsReactiveFactoryAttribute(IAttribute attribute) => 
        IsType(attribute, "ReactiveFactoryAttribute");

    public static bool HasFromServicesAttribute(IParameter parameter) => 
        parameter.Attributes.Any(a => IsType(a, "FromServicesAttribute"));

    public static bool HasSharedStateAttribute(IParameter parameter) => 
        parameter.Attributes.Any(a => IsType(a, "SharedStateAttribute"));

    public static bool HasBackingModelAttribute(IProperty property) => 
        property.Attributes.Any(IsBackingModelAttribute);
    
    public static bool IsBackingModelAttribute(IAttribute attribute) => 
        IsType(attribute, "BackingModelAttribute");
}