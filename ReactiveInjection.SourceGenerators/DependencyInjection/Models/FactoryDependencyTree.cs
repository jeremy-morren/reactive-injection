using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.SourceGenerators.DependencyInjection.Models;

internal class FactoryDependencyTree
{
    /// <summary>
    /// The containing factory type i.e. the wrapping ViewModelFactory
    /// </summary>
    public required IType FactoryType { get; init; }

    /// <summary>
    /// The view models to generate methods for
    /// </summary>
    public required IReadOnlyList<ViewModel> ViewModels { get; init; }

    /// <summary>
    /// The services required by <see cref="FactoryType"/>.
    /// These will be resolved via DI (i.e. constructor)
    /// </summary>
    public required IReadOnlyList<IType> Services { get; init; }

    /// <summary>
    /// The types that are shared between ViewModels i.e. stateful.
    /// These are implemented as fields on <see cref="FactoryType"/>
    /// </summary>
    public required IReadOnlyList<IType> SharedState { get; init; }
}