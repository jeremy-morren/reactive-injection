namespace ReactiveInjection.Abstractions;

/// <summary>
/// Indicates that the service should be resolved from the DI container
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromDIAttribute : Attribute
{
    
}