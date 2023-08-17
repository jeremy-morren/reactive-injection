using ReactiveInjection.Tokens;

// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace ReactiveInjection.Generator;

internal class FactoryDependencyTree
{
    /// <summary>
    /// The containing factory type i.e. the wrapping ViewModelFactory
    /// </summary>
    public required IType FactoryType { get; init; }

    public required ViewModel[] ViewModels { get; init; }

    /// <summary>
    /// The services required by <see cref="FactoryType"/>.
    /// These will be resolved via DI (i.e. constructor)
    /// </summary>
    public required IType[] Services { get; init; }

    /// <summary>
    /// The types that are shared between ViewModels i.e. stateful.
    /// These are implemented as fields on <see cref="FactoryType"/>
    /// </summary>
    public required IType[] SharedState { get; init; }
}

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