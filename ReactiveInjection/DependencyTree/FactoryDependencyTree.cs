using ReactiveInjection.Tokens;

// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace ReactiveInjection.DependencyTree;

internal class FactoryDependencyTree
{
    /// <summary>
    /// The containing factory type i.e. the type
    /// that will be registered into DI
    /// </summary>
    public IType FactoryType { get; set; }

    /// <summary>
    /// The services required by <see cref="FactoryType"/>.
    /// These will be resolved via DI (i.e. constructor)
    /// </summary>
    public IType[] Services { get; set; }

    /// <summary>
    /// The factory methods for generating ViewModels
    /// </summary>
    public FactoryMethod[] FactoryMethods { get; set; }

    /// <summary>
    /// The types that are shared between ViewModels i.e. stateful.
    /// These are implemented as fields on <see cref="FactoryType"/>
    /// </summary>
    public IType[] SharedState { get; set; }
}