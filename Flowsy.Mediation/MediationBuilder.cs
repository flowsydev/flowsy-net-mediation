using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Mediation;

/// <summary>
/// A builder for configuring the Mediation services.
/// </summary>
public class MediationBuilder
{
    private readonly IServiceCollection _services;
    
    internal MediationBuilder(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>
    /// Registers a request context provider and a behavior to resolve the request context.
    /// </summary>
    /// <param name="lifetime">
    /// The lifetime of the request context provider.
    /// </param>
    /// <typeparam name="TRequestContextProvider">
    /// The type of the request context provider.
    /// </typeparam>
    /// <returns>
    /// The MediationBuilder instance.
    /// </returns>
    public MediationBuilder UseRequestContext<TRequestContextProvider>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TRequestContextProvider : class, IRequestContextProvider
    {
        _services.Add(new ServiceDescriptor(typeof (IRequestContextProvider), typeof (TRequestContextProvider), lifetime));
        
        _services.Add(new ServiceDescriptor(
            typeof (IPipelineBehavior<,>),
            typeof (RequestContextResolutionBehavior<,>),
            lifetime
            ));
        
        return this;
    }

    /// <summary>
    /// Registers a request context provider and a behavior to resolve the request context.
    /// </summary>
    /// <param name="implementationFactory">
    /// A factory to create the request context provider.
    /// </param>
    /// <param name="lifetime">
    /// The lifetime of the request context provider.
    /// </param>
    /// <returns>
    /// The MediationBuilder instance.
    /// </returns>
    public MediationBuilder UseRequestContext(
        Func<IServiceProvider, IRequestContextProvider> implementationFactory,
        ServiceLifetime lifetime = ServiceLifetime.Transient
        )
    {
        _services.Add(new ServiceDescriptor(typeof (IRequestContextProvider), implementationFactory, lifetime));
        
        _services.Add(new ServiceDescriptor(
            typeof (IPipelineBehavior<,>),
            typeof (RequestContextResolutionBehavior<,>),
            lifetime
            ));
        
        return this;
    }
    
    /// <summary>
    /// Registers a behavior to validate requests.
    /// </summary>
    /// <param name="lifetime">
    /// The lifetime of the behavior.
    /// </param>
    /// <returns>
    /// The MediationBuilder instance.
    /// </returns>
    public MediationBuilder UseRequestValidation(ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        _services.Add(new ServiceDescriptor(typeof (IPipelineBehavior<,>), typeof (RequestValidationBehavior<,>), lifetime));
        return this;
    }
    
    /// <summary>
    /// Registers a behavior to log requests.
    /// </summary>
    /// <param name="lifetime">
    /// The lifetime of the behavior.
    /// </param>
    /// <returns>
    /// The MediationBuilder instance.
    /// </returns>
    public MediationBuilder UseRequestLogging(ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        _services.Add(new ServiceDescriptor(typeof (IPipelineBehavior<,>), typeof (RequestLoggingBehavior<,>), lifetime));
        return this;
    }
    
    /// <summary>
    /// Registers a custom pipeline behavior.
    /// </summary>
    /// <param name="behaviorType">
    /// The type of the behavior.
    /// </param>
    /// <param name="lifetime">
    /// The lifetime of the behavior.
    /// </param>
    /// <returns>
    /// The MediationBuilder instance.
    /// </returns>
    public MediationBuilder UseBehavior(Type behaviorType, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        _services.Add(new ServiceDescriptor(typeof (IPipelineBehavior<,>), behaviorType, lifetime));
        return this;
    }
}