using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Flowsy.Mediation;

/// <summary>
/// Validates user requests before being processed.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResult">The type of the expected result.</typeparam>
public class RequestValidationBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : Request<TResult>
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
/// <typeparam name="TRequest">The type of request.</typeparam>
public class RequestValidationBehavior<TRequest> : RequestValidationBehavior<TRequest, Unit>
    where TRequest : Request
{
    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators) : base(validators)
    {
    }
}
