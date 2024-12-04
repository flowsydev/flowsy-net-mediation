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
    /// <param name="configure">An action to configure a MediatRServiceConfiguration instance.</param>
    /// <returns>
    /// A MediationBuilder instance.
    /// </returns>
    public static MediationBuilder AddMediation(this IServiceCollection services, Action<MediatRServiceConfiguration> configure)
    {
        services.AddMediatR(configure);
        return new MediationBuilder(services);
    }
}