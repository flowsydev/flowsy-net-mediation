using MediatR;

namespace Flowsy.Mediation;

/// <summary>
/// Represents a request that carries a context object.
/// </summary>
public interface IContextualRequest : IBaseRequest;

/// <summary>
/// Represents a request that carries a context object.
/// </summary>
/// <typeparam name="TContext">
/// The type of the context object.
/// </typeparam>
public interface IContextualRequest<out TContext> : IContextualRequest
{
    /// <summary>
    /// The context object.
    /// </summary>
    TContext Context { get; }
}