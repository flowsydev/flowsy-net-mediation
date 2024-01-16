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

    public MediationBuilder UseRequestContext<TContext, TRequestContextProvider>()
        where TRequestContextProvider : class, IRequestContextProvider<TContext>
    {
        _services.AddScoped<IRequestContextProvider<TContext>, TRequestContextProvider>();
        _services.AddTransient(typeof (IPipelineBehavior<,>), typeof (RequestContextResolutionBehavior<,>));
        return this;
    }

    public MediationBuilder UseRequestContext<TContext>(Func<IServiceProvider, IRequestContextProvider<TContext>> implementationFactory)
    {
        _services.AddScoped(implementationFactory);
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