using JetBrains.Annotations;

namespace ReactiveInjection;

/// <summary>
/// Specifies that a factory should be constructed for <see cref="ViewModelFactory"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
[PublicAPI, MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
public sealed class ReactiveFactoryAttribute : Attribute
{
    /// <summary>
    /// The type on which to generate a factory method
    /// </summary>
    public Type ViewModelFactory { get; }

    /// <summary>
    /// Specifies that a factory method be added to <see cref="ViewModelFactory"/>
    /// </summary>
    /// <param name="viewModelFactory">The type to add a factory method to</param>
    public ReactiveFactoryAttribute(Type viewModelFactory) => ViewModelFactory = viewModelFactory;
}