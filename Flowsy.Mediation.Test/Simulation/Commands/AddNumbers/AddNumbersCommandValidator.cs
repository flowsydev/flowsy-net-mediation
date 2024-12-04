using FluentValidation;

namespace Flowsy.Mediation.Test.Simulation.Commands.AddNumbers;

public class AddNumbersCommandValidator : ApplicationRequestValidator<AddNumbersCommand>
{
    public AddNumbersCommandValidator() : base(false)
    {
        RuleFor(command => command.FirstNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage("First number must be greater than or equal to 0.");
        
        RuleFor(command => command.SecondNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Second number must be greater than or equal to 0.");
    }
}