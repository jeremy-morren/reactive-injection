namespace ReactiveInjection.Tokens;

internal interface IAssembly : IEquatable<IAssembly>
{
    string Name { get; }
}