using MediatR;

namespace Flowsy.Mediation;

/// <summary>
/// Represents a handler associated to a specific type of user request wich produces some result.
/// </summary>
/// <typeparam name="TRequest">The type of user request.</typeparam>
/// <typeparam name="TResult">The type of the expected result.</typeparam>
public abstract class RequestHandler<TRequest, TResult> : IRequestHandler<TRequest, TResult> 
    where TRequest : IRequest<TResult>
{
    async Task<TResult> IRequestHandler<TRequest, TResult>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request, cancellationToken);
        return await HandleAsync(request, cancellationToken);
    }

    /// <summary>
    /// Validates the data of the request being processed.
    /// </summary>
    /// <param name="request">The current request.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns></returns>
    protected virtual Task ValidateAsync(TRequest request, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    /// <summary>
    /// Processes the user request.
    /// </summary>
    /// <param name="request">The current request.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The expected result for the current request.</returns>
    protected virtual Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}

public abstract class RequestHandler<TRequest> : RequestHandler<TRequest, Unit>, IRequestHandler<TRequest, Unit>
    where TRequest : IRequest<Unit>
{
    async Task<Unit> IRequestHandler<TRequest, Unit>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request, cancellationToken);
        await HandleAsync(request, cancellationToken);
        return Unit.Value;
    }

    /// <summary>
    /// Processes the user request.
    /// </summary>
    /// <param name="request">The current request.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    protected new virtual Task HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}