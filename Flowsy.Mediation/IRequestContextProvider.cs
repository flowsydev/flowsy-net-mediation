namespace Flowsy.Mediation;

/// <summary>
/// Provides context objects for requests.
/// </summary>
public interface IRequestContextProvider
{
    /// <summary>
    /// Provides a context object for the specified request.
    /// </summary>
    /// <param name="request">
    /// The request.
    /// </param>
    /// <typeparam name="TRequest">
    /// The type of the request.
    /// </typeparam>
    /// <returns>
    /// The context object.
    /// </returns>
    object ProvideContext<TRequest>(TRequest request) where TRequest : IContextualRequest;
    
    /// <summary>
    /// Provides a context object for the specified request asynchronously.
    /// </summary>
    /// <param name="request">
    /// The request.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <typeparam name="TRequest">
    /// The type of the request.
    /// </typeparam>
    /// <returns>
    /// The context object.
    /// </returns>
    Task<object> ProvideContextAsync<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IContextualRequest;
}