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

    public MediationBuilder AddLogging()
    {
        _services.AddTransient(typeof (IPipelineBehavior<,>), typeof (LoggingBehavior<,>));
        return this;
    }
}