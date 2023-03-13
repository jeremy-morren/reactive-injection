using ReactiveInjection.Tokens;

namespace ReactiveInjection.DependencyTree;

#pragma warning disable CS8618
internal class FactoryMethod
{
    /// <summary>
    /// The underlying method
    /// </summary>
    public IMethod Method { get; init; }

    /// <summary>
    /// The factory return type
    /// </summary>
    public IType ReturnType { get; init; }
    
    /// <summary>
    /// The constructor on <see cref="ReturnType"/>
    /// to use for Dependency Injection
    /// </summary>
    public IConstructor Constructor { get; init; }
}