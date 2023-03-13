// ReSharper disable ReturnTypeCanBeEnumerable.Global
namespace ReactiveInjection.Tokens;

internal interface IParameter : IToken
{
    public string Name { get; }

    /// <summary>
    /// The 0-based index of the parameter
    /// </summary>
    public int Position { get; }

    public IType ParameterType { get; }

    public IAttribute[] GetCustomAttributes();
}