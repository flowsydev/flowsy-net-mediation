using FluentValidation;

namespace Flowsy.Mediation.Test.Simulation.Commands.MultiplyNumbers;

public class MultiplyNumbersCommandValidator : ApplicationRequestValidator<MultiplyNumbersCommand>
{
    public MultiplyNumbersCommandValidator()
    {
        RuleFor(command => command.FirstNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage("First number must be greater than or equal to 0.");
        
        RuleFor(command => command.SecondNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Second number must be greater than or equal to 0.");
    }
}