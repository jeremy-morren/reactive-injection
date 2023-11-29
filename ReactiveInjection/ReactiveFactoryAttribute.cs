namespace ReactiveInjection;

/// <summary>
/// Specifies that a factory should be constructed for <see cref="ViewModel"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class ReactiveFactoryAttribute : Attribute
{
    /// <summary>
    /// The ViewModel to be constructed
    /// </summary>
    public Type ViewModel { get; }

    /// <summary>
    /// Specifies that a factory should be constructed for <see cref="ViewModel"/>
    /// </summary>
    /// <param name="viewModel">The view model to generate a factory for</param>
    public ReactiveFactoryAttribute(Type viewModel) => ViewModel = viewModel;
}