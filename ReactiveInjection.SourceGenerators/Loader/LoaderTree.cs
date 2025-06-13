using JetBrains.Annotations;
using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.SourceGenerators.Loader;

internal class LoaderTree
{
    public required IAssembly Assembly { get; init; }
    
    public required List<ViewModelLoaderRoute> Routes { get; init; }
    
    public required List<IType> Services { get; init; }

    public string Filename => $"{Assembly.Name}.{LoaderManagerWriter.ClassName}.g.cs";
}

internal class ViewModelLoaderRoute
{
    public required IType ViewModel { get; init; }
    
    public required IMethod Method { get; init; }
    
    public required string RouteTemplate { get; init; }
    
    /// <summary>
    /// List of either <c>string</c> or <see cref="RouteSegmentParameter"/>
    /// </summary>
    public required List<object> Segments { get; init; }
    
    /// <summary>
    /// Parameters that are loaded from the query string
    /// </summary>
    public required List<IParameter> QueryParameters { get; init; }
}

internal class RouteSegmentParameter
{
    public required string Name { get; init; }
    
    /// <summary>
    /// Index of the parameter in method segments
    /// </summary>
    public required int Index { get; init; }

    /// <summary>
    /// The route parameter type
    /// </summary>
    public RouteParameterType Type { get; set; } = RouteParameterType.Unknown;

    /// <summary>
    /// Actual type of <see cref="Type"/>, unless <see cref="Type"/> is <see cref="RouteParameterType.String"/>
    /// </summary>
    public IType? TypeSymbol { get; set; }
    
    /// <summary>
    /// If the parameter is optional. Only the last parameter can be optional
    /// </summary>
    public required bool Optional { get; init; }
    
    public bool NameEquals(string name) => Name.Equals(name, StringComparison.OrdinalIgnoreCase);
}

[PublicAPI]
internal enum RouteParameterType
{
    /// <summary>
    /// The parameter type is unknown
    /// </summary>
    Unknown,
    
    /// <summary>
    /// Parameter is a string
    /// </summary>
    String,
    
    /// <summary>
    /// Parameter is a primitive type that can be parsed
    /// </summary>
    Primitive,
    
    /// <summary>
    /// Parameter is a type that implements IParsable
    /// </summary>
    Parsable,
}