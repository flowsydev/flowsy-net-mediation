namespace Flowsy.Mediation.Test.Simulation.Commands.AddNumbers;

public class AddNumbersCommandHandler : ApplicationRequestHandler<AddNumbersCommand, AddNumbersCommandResult>
{
    protected override Task<AddNumbersCommandResult> HandleAsync(AddNumbersCommand request, CancellationToken cancellationToken)
    {
        var value = request.FirstNumber + request.SecondNumber;
        
        return Task.FromResult(new AddNumbersCommandResult(value, request.Context.UserEmail));
    }
}