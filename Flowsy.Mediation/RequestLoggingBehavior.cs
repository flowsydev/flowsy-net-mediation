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
/// <typeparam name="TResponse">The type of the expected response.</typeparam>
public sealed class RequestLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<RequestLoggingBehavior<TRequest, TResponse>> _logger;

    public RequestLoggingBehavior(ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Pipeline handler. Perform any additional behavior and await the next delegate as necessary
    /// </summary>
    /// <param name="request">
    /// The request to handle.
    /// </param>
    /// <param name="next">
    /// Awaitable delegat for the next action in the pipeline. 
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the response obtained by handling the request.
    /// </returns>
    async Task<TResponse> IPipelineBehavior<TRequest, TResponse>.Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;
        var cultureName = CultureInfo.CurrentUICulture.Name;

        _logger.LogInformation(
            "Executing request {RequestName} ({Culture}){NewLine}{@Request}",
            requestName,
            cultureName,
            Environment.NewLine,
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
                    "Request {RequestName} ({Culture}) executed in {ElapsedTime:N} ms with result of type {ResultType}{NewLine}{@Result}", 
                    requestName, 
                    cultureName,
                    stopwatch.ElapsedMilliseconds,
                    resultType?.Name,
                    Environment.NewLine,
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
