using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text.Json.Serialization;
using MediatR;

namespace Flowsy.Mediation;

/// <summary>
/// Represents a user request to perform a task which produces some result.
/// </summary>
/// <typeparam name="TResult">The type of the expected result.</typeparam>
[Serializable]
public class Request<TResult> : IRequest<TResult>
{
    /// <summary>
    /// The current user.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public ClaimsPrincipal? User { get; set; }
    
    /// <summary>
    /// The current culture.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public CultureInfo Culture { get; set; } = CultureInfo.CurrentUICulture;

    /// <summary>
    /// Request data.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public IDictionary<string, object?> Data { get; } = new Dictionary<string, object?>();
}

/// <summary>
/// Represents a user request to perform a task with no expected result.
/// </summary>
public class Request : Request<Unit>, IRequest
{
}
