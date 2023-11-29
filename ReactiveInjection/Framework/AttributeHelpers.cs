using ReactiveInjection.Symbols;

namespace ReactiveInjection.Framework;

internal static class AttributeHelpers
{
    private static bool IsType(IAttribute attribute, string name)
    {
        return attribute.Type.Name.Equals(name, StringComparison.Ordinal);
    }
    
    private static bool IsFromServicesAttribute(IAttribute a) => 
        IsType(a, "FromServicesAttribute");
    
    private static bool IsSharedStateAttribute(IAttribute a) => 
        IsType(a, "SharedStateAttribute");
    
    public static bool IsReactiveFactoryAttribute(IAttribute attribute) => 
        IsType(attribute, "ReactiveFactoryAttribute");

    public static bool HasFromServicesAttribute(IParameter parameter) =>
        parameter.Attributes.Any(IsFromServicesAttribute);

    public static bool HasSharedStateAttribute(IParameter parameter) =>
        parameter.Attributes.Any(IsSharedStateAttribute);

    public static bool HasFromServicesAttribute(IProperty property) =>
        property.Attributes.Any(IsFromServicesAttribute);

    public static bool HasSharedStateAttribute(IProperty property) =>
        property.Attributes.Any(IsSharedStateAttribute);
    
    public static bool HasBackingModelAttribute(IProperty property) => 
        property.Attributes.Any(IsBackingModelAttribute);
    
    public static bool IsBackingModelAttribute(IAttribute attribute) => 
        IsType(attribute, "BackingModelAttribute");
}