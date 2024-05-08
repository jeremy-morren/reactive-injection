using JetBrains.Annotations;
using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.SourceGenerators.Routing;

internal class RoutingTree
{
    public required IAssembly Assembly { get; init; }
    
    public required List<ViewModelLoaderRoute> Routes { get; init; }
    
    public required List<IType> Services { get; init; }

    public string Filename => $"{Assembly.Name}.{RoutingManagerWriter.ClassName}.g.cs";
}

internal class ViewModelLoaderRoute
{
    public required IType ViewModel { get; init; }
    
    public required IMethod Method { get; init; }
    
    public required string RouteTemplate { get; init; }
    
    /// <summary>
    /// List of either <c>string</c> or <see cref="RouteParameter"/>
    /// </summary>
    public required List<object> Segments { get; init; }
}

internal class RouteParameter
{
    public required string Name { get; init; }
    
    /// <summary>
    /// Index of the parameter in method segments
    /// </summary>
    public required int Index { get; init; }
    
    public required RouteParameterType Type { get; init; }

    public bool NameEquals(string name) => Name.Equals(name, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// CSharp name of <see cref="Type"/>
    /// </summary>
    public string CSharpType => Type.ToString().ToLowerInvariant();
}

[PublicAPI]
internal enum RouteParameterType
{
    String,Int,Bool,Decimal
}