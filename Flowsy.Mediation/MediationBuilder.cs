using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Mediation;

public class MediationBuilder
{
    private readonly IServiceCollection _services;
    
    internal MediationBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public MediationBuilder UseBehavior(Type behaviorType)
    {
        _services.AddTransient(typeof (IPipelineBehavior<,>), behaviorType);
        return this;
    }

    public MediationBuilder UseRequestContext<TRequestContextProvider>()
        where TRequestContextProvider : class, IRequestContextProvider
    {
        _services.AddTransient<IRequestContextProvider, TRequestContextProvider>();
        _services.AddTransient(typeof (IPipelineBehavior<,>), typeof (RequestContextResolutionBehavior<,>));
        return this;
    }

    public MediationBuilder UseRequestContext(Func<IServiceProvider, IRequestContextProvider> implementationFactory)
    {
        _services.AddTransient(implementationFactory);
        _services.AddTransient(typeof (IPipelineBehavior<,>), typeof (RequestContextResolutionBehavior<,>));
        return this;
    }

    public MediationBuilder UseRequestValidation()
    {
        _services.AddTransient(typeof (IPipelineBehavior<,>), typeof (RequestValidationBehavior<,>));
        return this;
    }

    public MediationBuilder UseRequestLogging()
    {
        _services.AddTransient(typeof (IPipelineBehavior<,>), typeof (RequestLoggingBehavior<,>));
        return this;
    }
}