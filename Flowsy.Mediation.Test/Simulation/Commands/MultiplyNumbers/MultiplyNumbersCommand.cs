namespace Flowsy.Mediation.Test.Simulation.Commands.MultiplyNumbers;

public class MultiplyNumbersCommand : ApplicationRequest<MultiplyNumbersCommandResult>
{
    public MultiplyNumbersCommand()
    {
    }

    public MultiplyNumbersCommand(int firstNumber, int secondNumber)
    {
        FirstNumber = firstNumber;
        SecondNumber = secondNumber;
    }

    public int FirstNumber { get; set; }
    public int SecondNumber { get; set; }
}