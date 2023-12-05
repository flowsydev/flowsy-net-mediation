using MediatR;

namespace Flowsy.Mediation;

public abstract class AbstractRequestHandler<TRequest, TResult> : IRequestHandler<TRequest, TResult>
    where TRequest : AbstractRequest<TResult>
{
    Task<TResult> IRequestHandler<TRequest, TResult>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        return HandleAsync(request, cancellationToken);
    }

    protected virtual Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public abstract class AbstractRequestHandler<TRequest>
    : AbstractRequestHandler<TRequest, Unit>, IRequestHandler<TRequest, Unit>, IRequestHandler<TRequest>
    where TRequest : AbstractRequest<Unit>, IRequest
{
    Task IRequestHandler<TRequest>.Handle(TRequest request, CancellationToken cancellationToken)
        => HandleAsync(request, cancellationToken);
    
    async Task<Unit> IRequestHandler<TRequest, Unit>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        await HandleAsync(request, cancellationToken);
        return Unit.Value;
    }
    
    protected new virtual Task HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
