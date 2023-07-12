namespace ReactiveInjection;

/// <summary>
/// Specifies that a factory should be constructed for a reactive service
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ReactiveFactoryAttribute : Attribute
{
    
}