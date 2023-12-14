using MediatR;

namespace Flowsy.Mediation;

public sealed class RequestEnvironmentResolutionBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : AbstractRequest<TResult>
{
    private readonly IRequestEnvironmentResolver? _requestEnvironmentResolver;

    public RequestEnvironmentResolutionBehavior(IRequestEnvironmentResolver? requestEnvironmentResolver)
    {
        _requestEnvironmentResolver = requestEnvironmentResolver;
    }

    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        if (_requestEnvironmentResolver is not null)
            request.Environment = await _requestEnvironmentResolver.ResolveAsync(cancellationToken);
        
        return await next();
    }
}

public sealed class RequestEnvironmentResolutionBehavior<TRequest> : IPipelineBehavior<TRequest, Unit>
    where TRequest : AbstractRequest
{
    private readonly IRequestEnvironmentResolver? _requestEnvironmentResolver;
    
    public RequestEnvironmentResolutionBehavior(IRequestEnvironmentResolver? requestEnvironmentResolver)
    {
        _requestEnvironmentResolver = requestEnvironmentResolver;
    }

    public async Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken)
    {
        if (_requestEnvironmentResolver is not null)
            request.Environment = await _requestEnvironmentResolver.ResolveAsync(cancellationToken);
        
        return await next();
    }
}
