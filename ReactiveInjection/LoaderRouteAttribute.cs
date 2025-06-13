using JetBrains.Annotations;
using ReactiveInjection.Loader;

namespace ReactiveInjection;

/// <summary>
/// Marks a method as a loader route
/// </summary>
/// <remarks>
/// An instance of <see cref="IReactiveLoaderManager"/> will be generated
/// with all methods marked with this attribute.
/// <para>
/// A special parameter <c>owner</c> can be added to the method
/// to receive the <c>owner</c> parameter from <see cref="ReactiveLoaderService.Load"/>
/// </para>
/// </remarks>
[PublicAPI, MeansImplicitUse(ImplicitUseTargetFlags.Itself)]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class LoaderRouteAttribute : Attribute
{
    [RouteTemplate]
    public string Route { get; }

    public LoaderRouteAttribute([RouteTemplate] string route)
    {
        Route = route;
    }
}