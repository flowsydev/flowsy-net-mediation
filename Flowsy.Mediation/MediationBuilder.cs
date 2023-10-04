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

    public MediationBuilder AddBehavior(Type behaviorType)
    {
        _services.AddTransient(typeof (IPipelineBehavior<,>), behaviorType);
        return this;
    }

    public MediationBuilder AddRequestUser()
    {
        _services.AddTransient(typeof (IPipelineBehavior<,>), typeof (RequestUserResolutionBehavior<,>));
        return this;
    }

    public MediationBuilder AddRequestUser<T>() where T : class, IRequestUserResolver
    {
        _services.AddTransient<IRequestUserResolver, T>();
        _services.AddTransient(typeof (IPipelineBehavior<,>), typeof (RequestUserResolutionBehavior<,>));
        return this;
    }

    public MediationBuilder AddRequestUser(IRequestUserResolver requestUserResolver)
        => AddRequestUser(_ => requestUserResolver);

    public MediationBuilder AddRequestUser(Func<IServiceProvider, IRequestUserResolver> implementationFactory)
    {
        _services.AddTransient(implementationFactory);
        _services.AddTransient(typeof (IPipelineBehavior<,>), typeof (RequestUserResolutionBehavior<,>));
        return this;
    }

    public MediationBuilder AddRequestValidation()
    {
        _services.AddTransient(typeof (IPipelineBehavior<,>), typeof (RequestValidationBehavior<,>));
        return this;
    }

    public MediationBuilder AddRequestLogging()
    {
        _services.AddTransient(typeof (IPipelineBehavior<,>), typeof (RequestLoggingBehavior<,>));
        return this;
    }
}