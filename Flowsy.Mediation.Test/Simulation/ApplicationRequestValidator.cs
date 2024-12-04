using FluentValidation;

namespace Flowsy.Mediation.Test.Simulation;

public abstract class ApplicationRequestValidator<TRequest> : AbstractValidator<TRequest>
    where TRequest : IContextualRequest<OperationContext>
{
    public ApplicationRequestValidator(bool validateContext = true)
    {
        if (!validateContext)
            return;
        
        RuleFor(request => request)
            .Custom((request, context) =>
            {
                if (!string.IsNullOrEmpty(request.Context.UserEmail))
                    return;
                
                context.AddFailure( nameof(request.Context), "Context user email is required.");
            });
    }
}