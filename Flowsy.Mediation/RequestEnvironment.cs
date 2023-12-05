using System.Security.Claims;

namespace Flowsy.Mediation;

public class RequestEnvironment
{
    public RequestEnvironment(ClaimsPrincipal user)
    {
        User = user;
    }

    public ClaimsPrincipal User { get; }
}