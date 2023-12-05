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

    public MediationBuilder UseRequestEnvironment<T>() where T : class, IRequestEnvironmentResolver
    {
        _services.AddTransient<IRequestEnvironmentResolver, T>();
        _services.AddTransient(typeof (IPipelineBehavior<,>), typeof (RequestEnvironmentResolutionBehavior<,>));
        return this;
    }

    public MediationBuilder UseRequestEnvironment(Func<IServiceProvider, IRequestEnvironmentResolver> implementationFactory)
    {
        _services.AddTransient(implementationFactory);
        _services.AddTransient(typeof (IPipelineBehavior<,>), typeof (RequestEnvironmentResolutionBehavior<,>));
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