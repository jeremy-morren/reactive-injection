using ReactiveInjection.Reflection;

namespace ReactiveInjection.DependencyTree;

public record DependencyTree
{
    /// <summary>
    /// The containing factory type i.e. the type
    /// that will be registered into DI
    /// </summary>
    public required IType FactoryType { get; init; }

    /// <summary>
    /// The services required by <see cref="FactoryType"/>.
    /// These will be resolved via DI (i.e. constructor)
    /// </summary>
    public required IType[] RequiredServices { get; init; }

    /// <summary>
    /// The factory methods for generating ViewModels
    /// </summary>
    public required IMethod[] FactoryMethods { get; init; }

    /// <summary>
    /// The types that are shared between ViewModels i.e. stateful.
    /// These are implemented as Subject properties on <see cref="FactoryType"/>
    /// </summary>
    public required IType[] SharedState { get; init; }
}