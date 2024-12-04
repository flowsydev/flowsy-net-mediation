using System.Reflection;
using MediatR;

namespace Flowsy.Mediation;

/// <summary>
/// A behavior that resolves the context for a request.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the expected response.
/// </typeparam>
public sealed class RequestContextResolutionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IContextualRequest
{
    private readonly IRequestContextProvider? _requestContextProvider;

    public RequestContextResolutionBehavior(IRequestContextProvider? requestContextProvider)
    {
        _requestContextProvider = requestContextProvider;
    }

    /// <summary>
    /// Pipeline handler. Perform any additional behavior and await the next delegate as necessary
    /// </summary>
    /// <param name="request">
    /// The request to handle.
    /// </param>
    /// <param name="next">
    /// Awaitable delegat for the next action in the pipeline. 
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the response obtained by handling the request.
    /// </returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_requestContextProvider is null)
            return await next();
        
        var requestType = request.GetType();
        var contextProperty = requestType.GetProperty("Context", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (contextProperty is null || !contextProperty.CanWrite)
            return await next();
        
        var context = await _requestContextProvider.ProvideContextAsync(request, cancellationToken);
        if (ReferenceEquals(context, default) || context.GetType() != contextProperty.PropertyType)
            return await next();
        
        contextProperty.SetValue(request, context);
        
        return await next();
    }
}
