using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ReactiveInjection.Loader;

public static class ReactiveLoaderServiceCollectionExtensions
{
    public static IServiceCollection AddReactiveLoaderService(this IServiceCollection services)
    {
        services.TryAddTransient<ReactiveLoaderService>();
        
        var managers =
            from a in AppDomain.CurrentDomain.GetAssemblies()
            let manager = a.GetType("ReactiveInjection.Loader.ReactiveLoaderManager")
            where manager != null
            select new ServiceDescriptor(
                typeof(IReactiveLoaderManager),
                manager,
                ServiceLifetime.Transient);
        foreach (var manager in managers)
            services.TryAddEnumerable(manager);

        return services;
    }
}