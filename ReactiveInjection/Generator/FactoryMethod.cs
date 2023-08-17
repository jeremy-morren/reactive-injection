using ReactiveInjection.Tokens;

namespace ReactiveInjection.Generator;

#pragma warning disable CS8618
internal class FactoryMethod
{
    /// <summary>
    /// The underlying method
    /// </summary>
    public IMethod Method { get; set; }

    /// <summary>
    /// The factory return type
    /// </summary>
    public IType ReturnType { get; set; }
    
    /// <summary>
    /// The constructor on <see cref="ReturnType"/>
    /// to use for Dependency Injection
    /// </summary>
    public IConstructor Constructor { get; set; }
}