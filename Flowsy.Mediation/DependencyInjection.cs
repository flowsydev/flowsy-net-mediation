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
    /// This method also registers the InitializationBehavior service to set the current user and culture for every request. 
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <param name="assemblies">The assemblies to register requests and request handlers from.</param>
    /// <returns>The application service collection.</returns>
    public static IServiceCollection AddMediation(
        this IServiceCollection services,
        params Assembly[] assemblies
        )
        => services.AddMediation(true, true, assemblies);

    /// <summary>
    /// Registers assemblies containing requests and request handlers to trigger application processes.
    /// This method also registers the InitializationBehavior service to set the current user and culture for every request. 
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <param name="initializeRequests">Whether or not to register the InitializationBehavior service.</param>
    /// <param name="assemblies">The assemblies to register requests and request handlers from.</param>
    /// <returns>The application service collection.</returns>
    public static IServiceCollection AddMediation(
        this IServiceCollection services,
        bool initializeRequests,
        params Assembly[] assemblies
        )
        => services.AddMediation(initializeRequests, true, assemblies);

    /// <summary>
    /// Registers assemblies containing requests and request handlers to trigger application processes. 
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <param name="initializeRequests">Whether or not to register the InitializationBehavior service.</param>
    /// <param name="logRequests">Whether or not to register the LoggingBehavior service.</param>
    /// <param name="assemblies">The assemblies to register requests and request handlers from.</param>
    /// <returns>The application service collection.</returns>
    public static IServiceCollection AddMediation(
        this IServiceCollection services, 
        bool initializeRequests,
        bool logRequests,
        params Assembly[] assemblies
        )
    {
        services.AddMediatR(assemblies);
        
        if (initializeRequests)
            services.AddTransient(typeof (IPipelineBehavior<,>), typeof (InitializationBehavior<,>));
        
        if (logRequests)
            services.AddTransient(typeof (IPipelineBehavior<,>), typeof (LoggingBehavior<,>));
        
        return services;
    }
}