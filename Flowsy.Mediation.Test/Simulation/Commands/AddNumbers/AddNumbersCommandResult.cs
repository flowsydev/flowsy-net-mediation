namespace Flowsy.Mediation.Test.Simulation.Commands.AddNumbers;

public class AddNumbersCommandResult
{
    public AddNumbersCommandResult(int value, string addedBy)
    {
        Value = value;
        AddedBy = addedBy;
    }

    public int Value { get; }
    
    public string AddedBy { get; }
}