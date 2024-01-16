using System.Collections;
using System.Diagnostics;
using System.Globalization;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowsy.Mediation;

/// <summary>
/// Logs request and result information as processed.
/// </summary>
/// <typeparam name="TContext">The type of request context.</typeparam>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResult">The type of the expected result.</typeparam>
public sealed class RequestLoggingBehavior<TContext, TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : AbstractRequest<TContext, TResult>
{
    private readonly ILogger<RequestLoggingBehavior<TContext, TRequest, TResult>> _logger;

    public RequestLoggingBehavior(ILogger<RequestLoggingBehavior<TContext, TRequest, TResult>> logger)
    {
        _logger = logger;
    }

    async Task<TResult> IPipelineBehavior<TRequest, TResult>.Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;
        var cultureName = CultureInfo.CurrentUICulture.Name;

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(
                "Executing request {RequestName} ({CultureName})\n{@Request}",
                requestName,
                cultureName,
                request
            );
        }
        else
        {
            _logger.LogInformation(
                "Executing request {RequestName} ({Culture})\n{@Context}",
                requestName,
                cultureName,
                request.Context
            );
        }
        
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

/// <summary>
/// Logs request and result information as processed.
/// </summary>
/// <typeparam name="TContext">The type of request context.</typeparam>
/// <typeparam name="TRequest">The type of request.</typeparam>
public sealed class RequestLoggingBehavior<TContext, TRequest> : IPipelineBehavior<TRequest, Unit>
    where TRequest : AbstractRequest<TContext>
{
    private readonly ILogger<RequestLoggingBehavior<TContext, TRequest>> _logger;
    
    public RequestLoggingBehavior(ILogger<RequestLoggingBehavior<TContext, TRequest>> logger)
    {
        _logger = logger;
    }

    public async Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;
        var cultureName = CultureInfo.CurrentUICulture.Name;

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(
                "Executing request {RequestName} ({CultureName})\n{@Request}",
                requestName,
                cultureName,
                request
            );
        }
        else
        {
            _logger.LogInformation(
                "Executing request {RequestName} ({Culture})\n{@Context}",
                requestName,
                cultureName,
                request.Context
            );
        }
        
        var stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            var result = await next();
            stopwatch.Stop();

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(
                    "Request {RequestName} ({Culture}) executed in {ElapsedTime:N} ms with no result", 
                    requestName, 
                    cultureName,
                    stopwatch.ElapsedMilliseconds
                );
            }
            else
            {
                _logger.LogInformation(
                    "Request {RequestName} ({Culture}) executed in {ElapsedTime:N} ms with no result",
                    requestName,
                    cultureName,
                    stopwatch.ElapsedMilliseconds
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