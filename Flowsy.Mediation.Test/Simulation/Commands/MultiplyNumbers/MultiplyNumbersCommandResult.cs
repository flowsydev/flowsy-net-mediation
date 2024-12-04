namespace Flowsy.Mediation.Test.Simulation.Commands.MultiplyNumbers;

public class MultiplyNumbersCommandResult
{
    public MultiplyNumbersCommandResult(int value)
    {
        Value = value;
    }

    public int Value { get; }
}