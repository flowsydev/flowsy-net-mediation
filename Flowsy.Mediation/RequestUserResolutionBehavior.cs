using MediatR;

namespace Flowsy.Mediation;

/// <summary>
/// Sets the User property for every request.
/// For this to work, an instance of IRequestUserResolver must be registered in the Dependency Injection system.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResult">The type of the expected result.</typeparam>
public class RequestUserResolutionBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : Request<TResult>
{
    private readonly IRequestUserResolver? _userResolver;

    public RequestUserResolutionBehavior(IRequestUserResolver? userResolver = null)
    {
        _userResolver = userResolver;
    }

    async Task<TResult> IPipelineBehavior<TRequest, TResult>.Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        if (_userResolver is not null)
            request.User = await _userResolver.GetUserAsync<TRequest, TResult>(request, cancellationToken);
        
        return await next();
    }
}

/// <summary>
/// Sets the User property for every request.
/// For this to work, an instance of IRequestUserResolver must be registered in the Dependency Injection system.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
public class RequestUserResolutionBehavior<TRequest> : RequestUserResolutionBehavior<TRequest, Unit>
    where TRequest : Request
{
}