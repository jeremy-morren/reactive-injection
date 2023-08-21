using ReactiveInjection.Symbols;

namespace ReactiveInjection.ModelInjection;

internal class ViewModelDependencyTree
{
    /// <summary>
    /// The view model for which to
    /// </summary>
    public required IType ViewModel { get; init; }
    
    public required IType[] Models { get; init; }
}