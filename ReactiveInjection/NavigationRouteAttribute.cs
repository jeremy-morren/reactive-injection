using JetBrains.Annotations;

namespace ReactiveInjection;

[PublicAPI, MeansImplicitUse(ImplicitUseTargetFlags.Itself)]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class NavigationRouteAttribute : Attribute
{
    [RouteTemplate]
    public string Route { get; }

    public NavigationRouteAttribute([RouteTemplate] string route)
    {
        Route = route;
    }
}