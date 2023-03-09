using FluentValidation;
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
        if (_validators.Any())
            return await next();

        var validationContext = new ValidationContext<TRequest>(request);
        var validationFailures = _validators
            .Select(v => v.Validate(validationContext))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToArray();

        if (validationFailures.Any())
            throw new ValidationException(validationFailures);

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
