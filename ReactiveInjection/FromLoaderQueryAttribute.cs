using JetBrains.Annotations;

namespace ReactiveInjection;

/// <summary>
/// Marks that a parameter is optional, and should be loaded from the query string
/// </summary>
[PublicAPI, MeansImplicitUse(ImplicitUseTargetFlags.Itself)]
[AttributeUsage(AttributeTargets.Parameter)]
public class FromLoaderQueryAttribute : Attribute
{
    public string? Name { get; }
    public FromLoaderQueryAttribute() { }
    
    public FromLoaderQueryAttribute(string? name)
    {
        Name = name;
    }
}