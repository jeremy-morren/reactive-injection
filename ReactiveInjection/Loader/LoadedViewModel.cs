using JetBrains.Annotations;

namespace ReactiveInjection.Loader;

/// <summary>
/// A loaded view model
/// </summary>
/// <param name="ViewModel">The view model that was loaded by <paramref name="Loader"/></param>
/// <param name="Path">The path that was loaded</param>
/// <param name="Loader">The loader that matched to the route</param>
/// <param name="Elapsed">Time taken to load the view model</param>
[PublicAPI]
public record LoadedViewModel(object ViewModel, string Path, IReactiveViewModelLoader Loader, TimeSpan Elapsed);