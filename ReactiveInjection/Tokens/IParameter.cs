// ReSharper disable ReturnTypeCanBeEnumerable.Global
namespace ReactiveInjection.Tokens;

internal interface IParameter : IToken
{
    public string Name { get; }

    /// <summary>
    /// The 0-based index of the parameter
    /// </summary>
    public int Ordinal { get; }

    public IType Type { get; }

    public IAttribute[] GetCustomAttributes();
}