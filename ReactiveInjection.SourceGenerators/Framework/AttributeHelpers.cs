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

    private static bool IsFromParametersAttribute(IAttribute a) =>
        IsType(a, "FromParametersAttribute");
    
    private static bool IsSharedStateAttribute(IAttribute a) => 
        IsType(a, "SharedStateAttribute");
    
    public static bool IsReactiveFactoryAttribute(IAttribute attribute) => 
        IsType(attribute, "ReactiveFactoryAttribute");
    
    public static bool IsLoaderRouteAttribute(IAttribute attribute) => 
        IsType(attribute, "LoaderRouteAttribute");
    
    public static bool IsFromLoaderQueryAttribute(IAttribute a) =>
        IsType(a, "FromLoaderQueryAttribute");

    public static bool HasLoaderRouteAttribute(IMethod method) =>
        method.Attributes.Any(IsLoaderRouteAttribute);
    
    public static bool HasFromServicesAttribute(IParameter parameter) =>
        parameter.Attributes.Any(IsFromServicesAttribute);

    public static bool HasFromParametersAttribute(IParameter parameter) =>
        parameter.Attributes.Any(IsFromParametersAttribute);

    public static bool HasSharedStateAttribute(IParameter parameter) =>
        parameter.Attributes.Any(IsSharedStateAttribute);
    
    public static bool HasFromLoaderQueryAttribute(IParameter parameter) =>
        parameter.Attributes.Any(IsFromLoaderQueryAttribute);
}