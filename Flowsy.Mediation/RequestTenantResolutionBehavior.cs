using MediatR;

namespace Flowsy.Mediation;

/// <summary>
/// Sets the TenantId property for every request.
/// For this to work, an instance of IRequestTenantResolver must be registered in the Dependency Injection system.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResult">The type of the expected result.</typeparam>
public class RequestTenantResolutionBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : Request<TResult>
{
    private readonly IRequestTenantResolver _requestTenantResolver;

    public RequestTenantResolutionBehavior(IRequestTenantResolver requestTenantResolver)
    {
        _requestTenantResolver = requestTenantResolver;
    }

    public Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        request.TenantId = _requestTenantResolver.GetTenantId();
        return next();
    }
}