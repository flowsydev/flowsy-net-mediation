using System.Security.Claims;

namespace Flowsy.Mediation;

/// <summary>
/// Resolves the current user for every request.
/// </summary>
public interface IRequestUserResolver
{
    /// <summary>
    /// Gets the current user, which must be resolved from the context of the current request.
    /// For example, for a web application, the current user should be the value of HttpContext.User.
    /// </summary>
    /// <returns></returns>
    Task<ClaimsPrincipal?> GetUserAsync();
}