using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Flowsy.Mediation;

/// <summary>
/// Validates user requests before being processed.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResponse">The type of the expected response.</typeparam>
public sealed class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
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
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var validationContext = new ValidationContext<TRequest>(request);

        var validationResults = new List<ValidationResult>();
        foreach (var validator in _validators)
            validationResults.Add(await validator.ValidateAsync(validationContext, cancellationToken));
        
        var errors = (
            from e in validationResults.SelectMany(r => r.Errors)
            where e is not null
            select e
        ).ToArray();
        
        if (errors.Any())
            throw new ValidationException(errors);

        return await next();
    }
}
