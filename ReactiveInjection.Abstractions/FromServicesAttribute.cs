namespace ReactiveInjection;

/// <summary>
/// The parameter is a service resolved from the IoC container
/// </summary>
/// <remarks>
/// The service is injected via the view model factory constructor
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromServicesAttribute : Attribute
{
    
}