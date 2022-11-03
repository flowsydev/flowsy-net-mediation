using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Mediation;

/// <summary>
/// Dependency injection extensions.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers assemblies containing requests and request handlers to trigger application processes. 
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <param name="logRequests">Whether or not to register the LoggingBehavior service.</param>
    /// <param name="assemblies">The assemblies to register requests and request handlers from.</param>
    /// <returns>The application service collection.</returns>
    public static IServiceCollection AddMediation(
        this IServiceCollection services,
        bool logRequests,
        params Assembly[] assemblies
        )
    {
        services.AddMediatR(assemblies);
        
        if (logRequests)
            services.AddTransient(typeof (IPipelineBehavior<,>), typeof (LoggingBehavior<,>));
        
        return services;
    }
}