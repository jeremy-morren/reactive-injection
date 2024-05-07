using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.SourceGenerators.DependencyInjection.Models;

internal class LoaderViewModel : ViewModel
{
    /// <summary>
    /// A method to load the viewmodel. Returns Task[ViewModel], name is Load/LoadAsync.
    /// </summary>
    public required IMethod Method { get; init; }
}