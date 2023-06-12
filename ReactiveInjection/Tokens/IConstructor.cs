namespace ReactiveInjection.Tokens;

internal interface IConstructor : IToken
{
    /// <summary>
    /// Gets the declaring type
    /// (i.e. the type which has this constructor)
    /// </summary>
    IType ContainingType { get; }
    
    IAttribute[] GetCustomAttributes();
    IParameter[] GetParameters();
}