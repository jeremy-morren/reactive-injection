using System.Text.RegularExpressions;
using ReactiveInjection.SourceGenerators.DependencyInjection;
using ReactiveInjection.SourceGenerators.Framework;
using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.SourceGenerators.Loader;

internal class LoaderTreeBuilder
{
    private readonly ErrorLogWriter _log;

    public LoaderTreeBuilder(IErrorLog log)
    {
        _log = new ErrorLogWriter(log);
    }
    
    public bool TryBuild(IEnumerable<IMethod> methods, out LoaderTree tree)
    {
        var routes = new List<ViewModelLoaderRoute>();
        var services = new HashSet<IType>();
        
        foreach (var method in methods)
        {
            var attributes = method.Attributes
                .Where(AttributeHelpers.IsLoaderRouteAttribute)
                .ToList();
            
            if (attributes.Count == 0) 
                continue; //Method not relevant

            var vm = method.ContainingType;

            if (!method.IsStatic)
            {
                _log.LoaderMustBeStatic(method);
                continue;
            }
            
            if (!ValidReturnType(method, vm))
            {
                _log.IncorrectRouteHandlerSignature(method);
                continue;
            }
            
            //Try create a loader route from the attribute & method
            foreach (var a in attributes)
            {
                if (!TryHandleRoute(a, out var route, out var segments)
                    || !ValidateMethodParameters(method, segments, out var svc, out var queryParams))
                    continue;
                
                services.AddRange(svc);
                routes.Add(new ViewModelLoaderRoute()
                {
                    ViewModel = vm, 
                    Method = method, 
                    RouteTemplate = route,
                    Segments = segments,
                    QueryParameters = queryParams
                });
            }
        }

        if (routes.Count == 0)
        {
            tree = null!;
            return false;
        }
        
        tree = new LoaderTree()
        {
            Assembly = routes[0].ViewModel.Assembly,
            Routes = routes,
            Services = services.ToList()
        };
        return true;
    }

    private static bool ValidReturnType(IMethod method, IType viewModel)
    {
        if (method.ReturnType == null) return false;
        if (method.ReturnType.Equals(viewModel)) return true;
        return method.ReturnType.IsTask(out var arg) && arg.Equals(viewModel);
    }

    /// <summary>
    /// Matches a route parameter in the form of {Name:Type}. An optional ? can be added at the end
    /// </summary>
    private static readonly Regex RouteParameterRegex = new(
        @"^{(?<Name>\w+)(?<Optional>\??)}$", 
        RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
    
    private bool TryHandleRoute(IAttribute attribute, 
        out string route, 
        out List<object> segments)
    {
        route = attribute.StringParameter;
        segments = new List<object>();

        var split = route.Split('/');
        for (var i = 0; i < split.Length; i++)
        {
            var segment = split[i];
            var parameter = RouteParameterRegex.Match(segment);
            
            if (parameter.Success)
            {
                var name = parameter.Groups["Name"].Value;
                var optional = parameter.Groups["Optional"].Length > 0;
                if (optional && i != split.Length - 1)
                {
                    _log.OptionalRouteParameterIsNotLastSegment(attribute, name);
                    return false;
                }
                //Route parameter
                segments.Add(new RouteSegmentParameter()
                {
                    Name = name,
                    Optional = optional,
                    Index = segments.Count
                });
            }
            else
            {
                //Raw string
                segments.Add(segment);
            }
        }

        var duplicates = segments.OfType<RouteSegmentParameter>()
            .Duplicates(p => p.Name)
            .ToList();
        if (duplicates.Count == 0) return true;

        foreach (var d in duplicates)
            _log.DuplicateRouteParameters(attribute, d);
        return false;
    }

    private bool ValidateMethodParameters(IMethod method, 
        IEnumerable<object> routeSegments, 
        out HashSet<IType> services,
        out List<IParameter> queryParameters)
    {
        //Ensure that all method parameters are provided either by the route or by the DI container

        var routeParams = routeSegments.OfType<RouteSegmentParameter>().ToList();

        var valid = true;
        services = new HashSet<IType>();
        queryParameters = new List<IParameter>();
        foreach (var p in method.Parameters)
        {
            if (p.Type.IsCancellationToken())
                continue;

            if (AttributeHelpers.HasFromParametersAttribute(p))
                continue; //Taken from parameters array

            if (AttributeHelpers.HasFromServicesAttribute(p))
            {
                services.Add(p.Type);
                continue;
            }

            if (AttributeHelpers.HasFromLoaderQueryAttribute(p))
            {
                queryParameters.Add(p);
                continue;
            }

            var routeParam = routeParams.FirstOrDefault(x => x.NameEquals(p.Name));
            if (routeParam != null)
            {
                valid = ValidateRouteParam(p, routeParam);
                continue;
            }
            valid = false;
            _log.RouteHandlerParameterNotProvided(p);
        }
        
        //Ensure all route parameters are provided by the method
        foreach (var p in routeParams.Where(p => p.Type == RouteParameterType.Unknown))
        {
            valid = false;
            _log.RouteSegmentParameterNotFoundOnMethod(method, p.Name);
        }

        return valid;
    }

    private bool ValidateRouteParam(IParameter p, RouteSegmentParameter route)
    {
        //Ensure that the route parameter type is a known type
        //Then set the route parameter type
        if (p.Type.CSharpName == "string")
        {
            //String is a known type, doesn't need to be parsed
            route.Type = RouteParameterType.String;
            return true;
        }
        
        //All other types are value types
        if (p.Type.IsReferenceType)
        {
            _log.UnknownRouteHandlerParameterType(p);
            return false;
        }
        var type = p.Type;
        if (route.Optional)
        {
            //Optional parameters must be nullable
            if (!type.IsNullable)
            {
                _log.OptionalRouteParameterNotNullable(p);
                return false;
            }
            type = type.GetUnderlyingType(); //Get the actual type
        }
        else
        {
            if (type.IsNullable)
            {
                _log.RequiredRouteParameterNullable(p);
                return false;
            }
        }
        route.TypeSymbol = type;
        //Type can be a primitive or IParsable
        if (type.IsPrimitive)
        {
            route.Type = RouteParameterType.Primitive;
            return true;
        }
        if (ImplementsIParseable(type) || IsStronglyTypedId(type))
        {
            route.Type = RouteParameterType.Parsable;
            return true;
        }
        
        _log.UnknownRouteHandlerParameterType(p);
        return true;
    }
    
    private static bool ImplementsIParseable(IType type)
    {
        return type.Interfaces.Any(t => t.CSharpName == $"global::System.IParsable<{type.CSharpName}>");
    }

    private static bool IsStronglyTypedId(IType type)
    {
        // For types generated by StronglyTypedId library, we won't know that they
        // are IParsable because source generators cannot see each other
        // So we need to check if the type has the StronglyTypedIdAttribute, 
        // and assume that it is IParsable
        const string attribute = "global::StronglyTypedIds.StronglyTypedIdAttribute";
        return type.Attributes.Any(a => a.Type.CSharpName == attribute);
    }
}