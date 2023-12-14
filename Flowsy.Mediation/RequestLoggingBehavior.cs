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
    where TRequest : AbstractRequest<TResult>
{
    private readonly ILogger<RequestLoggingBehavior<TRequest, TResult>> _logger;

    public RequestLoggingBehavior(ILogger<RequestLoggingBehavior<TRequest, TResult>> logger)
    {
        _logger = logger;
    }

    async Task<TResult> IPipelineBehavior<TRequest, TResult>.Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        var user = request.Environment.User;
        var userName = user.Identity?.Name ?? "unknown";
        var requestName = request.GetType().Name;
        var cultureName = CultureInfo.CurrentUICulture.Name;

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(
                "User {User} ({Culture}) is executing request {RequestName}\n{@RequestContent}",
                userName,
                cultureName,
                requestName,
                request
            );
        }
        else
        {
            _logger.LogInformation(
                "User {User} ({Culture}) is executing request {RequestName}",
                userName,
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

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                int? resultCount = null;
                if (result is IEnumerable enumerable)
                    resultCount = enumerable.AsQueryable().Cast<object>().Count();
                
                _logger.LogDebug(
                    "User {User} ({Culture}) executed request {RequestName} in {ElapsedTime} ms with result of type {ResultType} [ {@Result} ]",
                    userName,
                    cultureName,
                    requestName,
                    stopwatch.ElapsedMilliseconds,
                    resultType?.Name,
                    resultCount == null ? result : $"{resultCount} item(s)"
                );
            }
            else
            {
                _logger.LogInformation(
                    "User {User} ({Culture}) executed request {RequestName} in {ElapsedTime} ms with result of type {ResultType}",
                    userName,
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
public sealed class RequestLoggingBehavior<TRequest> : IPipelineBehavior<TRequest, Unit>
    where TRequest : AbstractRequest
{
    private readonly ILogger<RequestLoggingBehavior<TRequest>> _logger;
    
    public RequestLoggingBehavior(ILogger<RequestLoggingBehavior<TRequest>> logger)
    {
        _logger = logger;
    }

    public async Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken)
    {
        var user = request.Environment.User;
        var userName = user.Identity?.Name ?? "unknown";
        var requestName = request.GetType().Name;
        var cultureName = CultureInfo.CurrentUICulture.Name;
        
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(
                "User {User} ({Culture}) is executing request {RequestName}\n{@RequestContent}",
                userName,
                cultureName,
                requestName,
                request
                );
        }
        else
        {
            _logger.LogInformation(
                "User {User} ({Culture}) is executing request {RequestName}",
                userName,
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

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(
                    "User {User} ({Culture}) executed request {RequestName} in {ElapsedTime} ms with no result",
                    userName,
                    cultureName,
                    requestName,
                    stopwatch.ElapsedMilliseconds
                );
            }
            else
            {
                _logger.LogInformation(
                    "User {User} ({Culture}) executed request {RequestName} in {ElapsedTime} ms with no result",
                    userName,
                    cultureName,
                    requestName,
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