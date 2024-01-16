using MediatR;

namespace Flowsy.Mediation;

public abstract class AbstractRequest<TContext, TResult> : IRequest<TResult>
{
    public TContext Context { get; internal set; } = default!;
}

public abstract class AbstractRequest<TContext> : IRequest
{
    public TContext Context { get; internal set; } = default!;
}