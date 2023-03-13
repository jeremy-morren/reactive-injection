namespace ReactiveInjection.Tokens;

internal interface IConstructor : IToken
{
    IType DeclaringType { get; }
    
    IAttribute[] GetCustomAttributes();
    IParameter[] GetParameters();
}