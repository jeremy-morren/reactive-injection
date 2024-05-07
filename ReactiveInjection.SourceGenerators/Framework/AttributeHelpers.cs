using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.SourceGenerators.Framework;

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
}