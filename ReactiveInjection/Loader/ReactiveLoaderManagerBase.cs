using JetBrains.Annotations;

namespace ReactiveInjection.Loader;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract class ReactiveLoaderManagerBase
{
    /// <summary>
    /// Gets a parameter from the parameters array
    /// </summary>
    protected static T GetParameter<T>(object[] parameters, string paramName)
    {
        if (parameters == null) throw new ArgumentNullException(nameof(parameters));
        var result = parameters.OfType<T>().ToList();
        return result.Count switch
        {
            1 => result[0],
            0 => throw new ArgumentException(
                $"Could not provide parameter {paramName}. No parameter of type {typeof(T)} provided"),
            _ => throw new ArgumentException(
                $"Could not provide parameter {paramName}. Multiple parameters of type {typeof(T)} provided")
        };
    }
}