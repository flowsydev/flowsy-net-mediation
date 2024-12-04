using MediatR;

namespace Flowsy.Mediation;

/// <summary>
/// Represents a handler for a request that carries a context object.
/// </summary>
/// <typeparam name="TContext">
/// The type of the context required to process the request.
/// </typeparam>
/// <typeparam name="TRequest">
/// The type of the request to be processed.
/// </typeparam>
/// <typeparam name="TResult">
/// The type of the result of processing the request.
/// </typeparam>
public abstract class ContextualRequestHandler<TContext, TRequest, TResult> : IRequestHandler<TRequest, TResult>
    where TRequest : ContextualRequest<TContext, TResult>
{
    Task<TResult> IRequestHandler<TRequest, TResult>.Handle(TRequest request, CancellationToken cancellationToken)
        => HandleAsync(request, cancellationToken);

    /// <summary>
    /// Handles the request of type <typeparamref name="TRequest"/>.
    /// </summary>
    /// <param name="request">
    /// The request to be processed.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the result of processing the request.
    /// </returns>
    protected abstract Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Represents a handler for a request that carries a context object.
/// </summary>
/// <typeparam name="TContext">
/// The type of the context required to process the request.
/// </typeparam>
/// <typeparam name="TRequest">
/// The type of the request to be processed.
/// </typeparam>
public abstract class ContextualRequestHandler<TContext, TRequest> : IRequestHandler<TRequest> 
    where TRequest : ContextualRequest<TContext>
{
    Task IRequestHandler<TRequest>.Handle(TRequest request, CancellationToken cancellationToken)
        => HandleAsync(request, cancellationToken);

    /// <summary>
    /// Handles the request of type <typeparamref name="TRequest"/>.
    /// </summary>
    /// <param name="request">
    /// The request to be processed.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    protected abstract Task HandleAsync(TRequest request, CancellationToken cancellationToken);
}
