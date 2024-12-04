namespace Flowsy.Mediation.Test.Simulation.Commands.MultiplyNumbers;

public class MultiplyNumbersCommandHandler : ApplicationRequestHandler<MultiplyNumbersCommand, MultiplyNumbersCommandResult>
{
    protected override Task<MultiplyNumbersCommandResult> HandleAsync(MultiplyNumbersCommand request, CancellationToken cancellationToken)
    {
        var value = request.FirstNumber * request.SecondNumber;
        
        return Task.FromResult(new MultiplyNumbersCommandResult(value));
    }
}