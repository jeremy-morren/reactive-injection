namespace ReactiveInjection;

/// <summary>
/// Specifies that a property or field is model backing the current view model
/// </summary>
/// <remarks>
/// Properties will be added to the view model that are backed by the model.
/// The containing class must inherit from <c>ReactiveObject</c> from <c>ReactiveUI</c>
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class BackingModelAttribute : Attribute
{
    /// <summary>
    /// Properties to exclude from model generation
    /// </summary>
    public string[] ExcludeProps { get; }

    /// <summary>
    /// Specifies that a property or field is model backing the current view model
    /// </summary>
    /// <param name="excludeProps">Properties to exclude from model generation</param>
    public BackingModelAttribute(params string[] excludeProps)
    {
        ExcludeProps = excludeProps;
    }
}