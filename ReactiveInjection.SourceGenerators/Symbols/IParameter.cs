namespace ReactiveInjection.SourceGenerators.Symbols;

internal interface IParameter : IToken
{
    int Ordinal { get; }
    
    string Name { get; }
    
    IType Type { get; }

    IEnumerable<IAttribute> Attributes { get; }
}