namespace ReactiveInjection.Symbols;

internal interface IAssembly : IEquatable<IAssembly>
{
    string Name { get; }
}