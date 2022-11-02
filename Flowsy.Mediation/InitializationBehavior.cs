using System.Globalization;
using System.Security.Claims;
using MediatR;

namespace Flowsy.Mediation;

/// <summary>
/// Initializes a request with the current user and UI culture.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResult">The type of the expected result.</typeparam>
public class InitializationBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult> 
    where TRequest : Request<TResult>, IRequest<TResult>
{
    Task<TResult> IPipelineBehavior<TRequest, TResult>.Handle(
        TRequest request,
        RequestHandlerDelegate<TResult> next,
        CancellationToken cancellationToken
        )
    {
        request.User = ClaimsPrincipal.Current;
        request.Culture = CultureInfo.CurrentUICulture;
        return next();
    }
}

/// <summary>
/// Initializes a request with the current user and UI culture.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
public class InitializationBehavior<TRequest> : InitializationBehavior<TRequest, Unit>
    where TRequest : Request
{
}