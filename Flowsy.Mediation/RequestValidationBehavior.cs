using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Flowsy.Mediation;

/// <summary>
/// Validates user requests before being processed.
/// </summary>
/// <typeparam name="TContext">The type of request context.</typeparam>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResult">The type of the expected result.</typeparam>
public sealed class RequestValidationBehavior<TContext, TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : AbstractRequest<TContext, TResult>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
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

/// <summary>
/// Validates user requests before being processed.
/// </summary>
/// <typeparam name="TContext">The type of request context.</typeparam>
/// <typeparam name="TRequest">The type of request.</typeparam>
public sealed class RequestValidationBehavior<TContext, TRequest> : IPipelineBehavior<TRequest, Unit>
    where TRequest : AbstractRequest<TContext>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken)
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
