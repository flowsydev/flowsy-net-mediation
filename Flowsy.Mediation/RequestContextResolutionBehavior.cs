using MediatR;

namespace Flowsy.Mediation;

public sealed class RequestContextResolutionBehavior<TContext, TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : AbstractRequest<TContext, TResult>
{
    private readonly IRequestContextProvider<TContext>? _requestContextProvider;

    public RequestContextResolutionBehavior(IRequestContextProvider<TContext>? requestContextProvider = null)
    {
        _requestContextProvider = requestContextProvider;
    }

    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        if (_requestContextProvider is not null)
            request.Context = await _requestContextProvider.ProvideAsync(cancellationToken);
        
        return await next();
    }
}

public sealed class RequestContextResolutionBehavior<TContext, TRequest> : IPipelineBehavior<TRequest, Unit>
    where TRequest : AbstractRequest<TContext>
{
    private readonly IRequestContextProvider<TContext>? _requestContextProvider;
    
    public RequestContextResolutionBehavior(IRequestContextProvider<TContext>? requestContextProvider = null)
    {
        _requestContextProvider = requestContextProvider;
    }

    public async Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken)
    {
        if (_requestContextProvider is not null)
            request.Context = await _requestContextProvider.ProvideAsync(cancellationToken);
        
        return await next();
    }
}
