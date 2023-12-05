using System.Security.Claims;
using MediatR;

namespace Flowsy.Mediation;

public abstract record AbstractRequest<TResult> : IRequest<TResult>
{
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public RequestEnvironment Environment { get; internal set; } = new(ClaimsPrincipal.Current ?? new ClaimsPrincipal());
}

public abstract record AbstractRequest : AbstractRequest<Unit>, IRequest;