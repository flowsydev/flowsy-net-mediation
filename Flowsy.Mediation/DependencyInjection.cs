using System.Reflection;
using Flowsy.Localization;
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
    /// <param name="assemblies">The assemblies to register requests and request handlers from.</param>
    /// <returns>The application service collection.</returns>
    public static MediationBuilder AddMediation(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (!services.Any())
            throw new ArgumentException("NoAssemblyWasSpecified".Localize(), nameof(assemblies));
        
        services.AddMediatR(assemblies);
        
        return new MediationBuilder(services);
    }
}