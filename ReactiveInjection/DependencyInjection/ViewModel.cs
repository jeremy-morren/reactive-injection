using ReactiveInjection.Symbols;

namespace ReactiveInjection.DependencyInjection;

internal class ViewModel
{
    /// <summary>
    /// The view model type
    /// </summary>
    public required IType Type { get; init; }

    /// <summary>
    /// The constructor to use
    /// </summary>
    public required IConstructor Constructor { get; init; }

    /// <summary>
    /// The method parameters (i.e. not services or shared state)
    /// </summary>
    public required IParameter[] MethodParams { get; init; }
}