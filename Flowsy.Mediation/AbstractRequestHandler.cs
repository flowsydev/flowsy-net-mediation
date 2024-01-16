using MediatR;

namespace Flowsy.Mediation;

public abstract class AbstractRequestHandler<TContext, TRequest, TResult> : IRequestHandler<TRequest, TResult>
    where TRequest : AbstractRequest<TContext, TResult>
{
    Task<TResult> IRequestHandler<TRequest, TResult>.Handle(TRequest request, CancellationToken cancellationToken)
        => HandleAsync(request, cancellationToken);

    protected virtual Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public abstract class AbstractRequestHandler<TContext, TRequest> : IRequestHandler<TRequest> 
    where TRequest : AbstractRequest<TContext>
{
    Task IRequestHandler<TRequest>.Handle(TRequest request, CancellationToken cancellationToken)
        => HandleAsync(request, cancellationToken);

    protected virtual Task HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
