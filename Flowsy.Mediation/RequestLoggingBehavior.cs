using System.Collections;
using System.Diagnostics;
using MediatR;
using Serilog;
using Serilog.Events;

namespace Flowsy.Mediation;

/// <summary>
/// Logs request and result information as processed.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResult">The type of the expected result.</typeparam>
public class RequestLoggingBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : Request<TResult>
{
    private readonly ILogger _logger;

    public RequestLoggingBehavior()
    {
        _logger = Log.ForContext<RequestLoggingBehavior<TRequest, TResult>>();
    }
    
    async Task<TResult> IPipelineBehavior<TRequest, TResult>.Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        var userId = request.User?.Identity?.Name ?? "unknown";
        var requestName = request.GetType().Name;
        var cultureName = request.Culture.Name;

        if (_logger.IsEnabled(LogEventLevel.Debug))
        {
            _logger.Debug(
                "User {User} ({Culture}) is executing request {RequestName}\n{@RequestContent}",
                userId,
                cultureName,
                requestName,
                request
            );
        }
        else
        {
            _logger.Information(
                "User {User} ({Culture}) is executing request {RequestName}",
                userId,
                cultureName,
                requestName
            );
        }
        
        var stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            var result = await next();
            stopwatch.Stop();

            var resultType = result?.GetType();

            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                int? resultCount = null;
                if (result is IEnumerable enumerable)
                    resultCount = enumerable.AsQueryable().Cast<object>().Count();
                
                _logger.Debug(
                    "User {User} ({Culture}) executed request {RequestName} in {ElapsedTime} ms with result of type {ResultType} [ {@Result} ]",
                    userId,
                    cultureName,
                    requestName,
                    stopwatch.ElapsedMilliseconds,
                    resultType?.Name,
                    resultCount == null ? result : $"{resultCount} item(s)"
                );
            }
            else
            {
                _logger.Information(
                    "User {User} ({Culture}) executed request {RequestName} in {ElapsedTime} ms with result of type {ResultType}",
                    userId,
                    cultureName,
                    requestName,
                    stopwatch.ElapsedMilliseconds,
                    resultType?.Name
                );
            }

            return result;
        }
        catch (Exception)
        {
            if (stopwatch.IsRunning)
                stopwatch.Stop();

            throw;
        }
    }
}

/// <summary>
/// Logs request and result information as processed.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
public class RequestLoggingBehavior<TRequest> : RequestLoggingBehavior<TRequest, Unit>
    where TRequest : Request
{
}