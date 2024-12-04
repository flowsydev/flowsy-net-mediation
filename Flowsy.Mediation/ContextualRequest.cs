using MediatR;

namespace Flowsy.Mediation;

/// <summary>
/// A request that carries a context object.
/// </summary>
/// <typeparam name="TContext">
/// The type of the context object.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response.
/// </typeparam>
public abstract class ContextualRequest<TContext, TResponse> : IRequest<TResponse>, IContextualRequest<TContext>
{
    /// <summary>
    /// The context object.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public TContext Context { get; internal set; } = default!;
}

/// <summary>
/// A request that carries a context object.
/// </summary>
/// <typeparam name="TContext">
/// The type of the context object.
/// </typeparam>
public abstract class ContextualRequest<TContext> : IRequest, IContextualRequest<TContext>
{
    /// <summary>
    /// The context object.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public TContext Context { get; internal set; } = default!;
}