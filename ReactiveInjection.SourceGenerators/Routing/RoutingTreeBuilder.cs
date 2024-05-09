using System.Text.RegularExpressions;
using ReactiveInjection.SourceGenerators.DependencyInjection;
using ReactiveInjection.SourceGenerators.Framework;
using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.SourceGenerators.Routing;

internal class RoutingTreeBuilder
{
    private readonly ErrorLogWriter _log;

    public RoutingTreeBuilder(IErrorLog log)
    {
        _log = new ErrorLogWriter(log);
    }
    
    public bool TryBuild(IEnumerable<IMethod> methods, out RoutingTree tree)
    {
        var routes = new List<ViewModelLoaderRoute>();
        var services = new HashSet<IType>();
        
        foreach (var method in methods)
        {
            var attributes = method.Attributes
                .Where(AttributeHelpers.IsNavigationRouteAttribute)
                .ToList();
            
            if (attributes.Count == 0) 
                continue; //Method not relevant

            var vm = method.ContainingType;
            
            if (!ValidReturnType(method, vm))
            {
                _log.IncorrectRouteHandlerSignature(method);
                continue;
            }

            foreach (var a in attributes)
            {
                if (!TryHandleRoute(a, out var route, out var segments)
                    || !ValidateMethodParameters(method, segments, out var svc)) continue;
                
                services.AddRange(svc);
                routes.Add(new ViewModelLoaderRoute()
                {
                    ViewModel = vm, 
                    Method = method, 
                    RouteTemplate = route,
                    Segments = segments
                });
            }
        }

        if (routes.Count == 0)
        {
            tree = null!;
            return false;
        }
        
        tree = new RoutingTree()
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
    /// Matches a route parameter in the form of {Name:Type}
    /// </summary>
    private static readonly Regex RouteParameterRegex = new(@"^{(?<Name>\w+):?(?<Type>(?>(?>string)|(?>int)|(?>bool)|(?>decimal)))?}$", 
        RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
    
    private bool TryHandleRoute(IAttribute attribute, 
        out string route, 
        out List<object> segments)
    {
        route = attribute.StringParameter;
        segments = new List<object>();
        
        foreach (var segment in route.Split('/'))
        {
            var parameter = RouteParameterRegex.Match(segment);
            
            if (parameter.Success)
            {
                //Route parameter
                segments.Add(new RouteParameter()
                {
                    Name = parameter.Groups["Name"].Value,
                    Type = parameter.Groups["Type"].Success
                        ? ParseType(parameter.Groups["Type"].Value)
                        : RouteParameterType.String,
                    Index = segments.Count
                });
            }
            else
            {
                //Raw string
                segments.Add(segment);
            }
        }

        var duplicates = segments.OfType<RouteParameter>()
            .Duplicates(p => p.Name)
            .ToList();
        if (duplicates.Count == 0) return true;

        foreach (var d in duplicates)
            _log.DuplicateRouteParameters(attribute, d);
        return false;
    }

    private bool ValidateMethodParameters(IMethod method, IEnumerable<object> routeSegments, out HashSet<IType> services)
    {
        //Ensure that all method parameters are provided either by the route or by the DI container

        var routeParams = routeSegments.OfType<RouteParameter>().ToList();

        var valid = true;
        services = new HashSet<IType>();
        foreach (var p in method.Parameters)
        {
            if (p.Type.IsCancellationToken())
                continue;

            if (AttributeHelpers.HasFromServicesAttribute(p))
            {
                services.Add(p.Type);
                continue;
            }

            var routeParam = routeParams.FirstOrDefault(x => x.NameEquals(p.Name));
            if (routeParam != null)
            {
                if (Matches(p.Type, routeParam.Type))
                    continue;
                
                valid = false;
                _log.InvalidRouteHandlerParameterType(p, routeParam.Type);
                continue;
            }
            valid = false;
            _log.RouteHandlerParameterNotProvided(p);
        }

        return valid;
    }
    
    private static RouteParameterType ParseType(string type) =>
        (RouteParameterType)Enum.Parse(typeof(RouteParameterType), type, true);

    private static bool Matches(IType type, RouteParameterType parameterType)
    {
        return type.CSharpName.Equals(parameterType.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}