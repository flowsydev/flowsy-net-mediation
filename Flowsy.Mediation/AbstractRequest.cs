using MediatR;

namespace Flowsy.Mediation;

public abstract class AbstractRequest<TContext, TResult> : IRequest<TResult>
{
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public TContext Context { get; internal set; } = default!;
}

public abstract class AbstractRequest<TContext> : IRequest
{
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public TContext Context { get; internal set; } = default!;
}