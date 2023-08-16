namespace ReactiveInjection;

/// <summary>
/// The parameter is an object shared among all ViewModels
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class SharedStateAttribute : Attribute
{
    
}