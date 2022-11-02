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
public class LoggingBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : Request<TResult>
{
    private readonly ILogger _logger;

    public LoggingBehavior()
    {
        _logger = Log.ForContext<LoggingBehavior<TRequest, TResult>>();
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

            _logger.Information(
                "User {User} ({Culture}) executed request {RequestName} in {ElapsedTime} ms",
                userId,
                cultureName,
                requestName,
                stopwatch.ElapsedMilliseconds
            );

            if (!_logger.IsEnabled(LogEventLevel.Debug))
                return result;
            
            int? resultCount = null;
            if (result is IEnumerable enumerable)
                resultCount = enumerable.AsQueryable().Cast<object>().Count();
                
            _logger.Debug(
                "Request result: {@Result}",
                resultCount == null ? result : $"{resultCount} item(s)"
            );

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
public class LoggingBehavior<TRequest> : LoggingBehavior<TRequest, Unit>
    where TRequest : Request
{
}