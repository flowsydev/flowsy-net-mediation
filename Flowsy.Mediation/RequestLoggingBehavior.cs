using System.Collections;
using System.Diagnostics;
using System.Globalization;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowsy.Mediation;

/// <summary>
/// Logs request and result information as processed.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResult">The type of the expected result.</typeparam>
public sealed class RequestLoggingBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : notnull
{
    private readonly ILogger<RequestLoggingBehavior<TRequest, TResult>> _logger;

    public RequestLoggingBehavior(ILogger<RequestLoggingBehavior<TRequest, TResult>> logger)
    {
        _logger = logger;
    }

    async Task<TResult> IPipelineBehavior<TRequest, TResult>.Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;
        var cultureName = CultureInfo.CurrentUICulture.Name;

        _logger.LogInformation(
            "Executing request {RequestName} ({Culture})\n{@Request}",
            requestName,
            cultureName,
            request
            );
        
        var stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            var result = await next();
            stopwatch.Stop();

            var resultType = result?.GetType();

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                int? resultCount = null;
                if (result is IEnumerable enumerable)
                    resultCount = enumerable.AsQueryable().Cast<object>().Count();
                
                _logger.LogDebug(
                    "Request {RequestName} ({Culture}) executed in {ElapsedTime:N} ms with result of type {ResultType} [ {@Result} ]", 
                    requestName, 
                    cultureName,
                    stopwatch.ElapsedMilliseconds,
                    resultType?.Name,
                    resultCount.HasValue ? $"{resultCount.Value} item(s)" : result
                );
            }
            else
            {
                _logger.LogInformation(
                    "Request {RequestName} ({Culture}) executed in {ElapsedTime:N} ms with result of type {ResultType}",
                    requestName,
                    cultureName,
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
