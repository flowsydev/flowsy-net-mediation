using MediatR;

namespace Flowsy.Mediation;

public sealed class RequestContextResolutionBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : notnull
{
    private readonly IRequestContextProvider? _requestContextProvider;

    public RequestContextResolutionBehavior(IRequestContextProvider? requestContextProvider = null)
    {
        _requestContextProvider = requestContextProvider;
    }

    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        if (_requestContextProvider is null)
            return await next();
        
        var requestType = typeof(TRequest);
        var property = requestType.GetProperty("Context");
        if (property is null)
            return await next();
        
        var context = await _requestContextProvider.ProvideContextAsync(cancellationToken);
        if (property.PropertyType == context.GetType())
            property.SetValue(request, context);
        
        return await next();
    }
}
