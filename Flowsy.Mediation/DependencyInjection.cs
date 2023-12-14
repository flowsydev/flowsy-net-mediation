using System.Reflection;
using Flowsy.Mediation.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Mediation;

/// <summary>
/// Dependency injection extensions.
/// </summary>
public static class DependencyInjection
{
    public static MediationBuilder AddMediation(
        this IServiceCollection services,
        params Assembly[] assemblies
        )
        => services.AddMediation(null, assemblies);

    /// <summary>
    /// Registers assemblies containing requests and request handlers to trigger application processes. 
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <param name="configure">An action con configure an MediatRServiceConfiguration instance.</param>
    /// <param name="assemblies">The assemblies to register requests and request handlers from.</param>
    /// <returns>The application service collection.</returns>
    public static MediationBuilder AddMediation(
        this IServiceCollection services,
        Action<MediatRServiceConfiguration>? configure,
        params Assembly[] assemblies
        )
    {
        if (!assemblies.Any())
            throw new ArgumentException(Strings.NoAssemblyWasSpecified, nameof(assemblies));
        
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblies(assemblies);
            configure?.Invoke(configuration);
        });
        
        return new MediationBuilder(services);
    }
}