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

    public MediationBuilder AddRequestUser(Func<IServiceProvider, IRequestUserResolver> implementationFactory)
    {
        _services.AddTransient<IRequestUserResolver>(implementationFactory);
        _services.AddTransient(typeof (IPipelineBehavior<,>), typeof (RequestUserResolutionBehavior<,>));
        return this;
    }

    public MediationBuilder AddLogging()
    {
        _services.AddTransient(typeof (IPipelineBehavior<,>), typeof (LoggingBehavior<,>));
        return this;
    }
}