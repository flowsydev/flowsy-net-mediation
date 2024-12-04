namespace Flowsy.Mediation.Test.Simulation.Commands.AddNumbers;

public class AddNumbersCommand : ApplicationRequest<AddNumbersCommandResult>
{
    public AddNumbersCommand()
    {
    }

    public AddNumbersCommand(int firstNumber, int secondNumber)
    {
        FirstNumber = firstNumber;
        SecondNumber = secondNumber;
    }

    public int FirstNumber { get; set; }
    public int SecondNumber { get; set; }
}