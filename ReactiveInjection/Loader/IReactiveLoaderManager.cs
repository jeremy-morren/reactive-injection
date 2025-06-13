namespace ReactiveInjection.Loader;

public interface IReactiveLoaderManager
{
    IReadOnlyList<IReactiveViewModelLoader> Loaders { get; }
}