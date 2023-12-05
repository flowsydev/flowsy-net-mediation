using MediatR;

namespace Flowsy.Mediation;

public class RequestEnvironmentResolutionBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : AbstractRequest<TResult>
{
    private readonly IRequestEnvironmentResolver? _requestEnvironmentResolver;

    public RequestEnvironmentResolutionBehavior(IRequestEnvironmentResolver? requestEnvironmentResolver)
    {
        _requestEnvironmentResolver = requestEnvironmentResolver;
    }

    public virtual async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        if (_requestEnvironmentResolver is not null)
            request.Environment = await _requestEnvironmentResolver.ResolveAsync(cancellationToken);
        
        return await next();
    }
}

public class RequestEnvironmentResolutionBehavior<TRequest> : RequestEnvironmentResolutionBehavior<TRequest, Unit>
    where TRequest : AbstractRequest
{
    public RequestEnvironmentResolutionBehavior(IRequestEnvironmentResolver requestEnvironmentResolver) : base(requestEnvironmentResolver)
    {
    }
}
