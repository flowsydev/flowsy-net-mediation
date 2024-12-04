namespace Flowsy.Mediation.Test.Simulation;

public class OperationContext
{
    public static readonly OperationContext Empty = new OperationContext(string.Empty);
    
    public OperationContext(string userEmail)
    {
        UserEmail = userEmail;
    }

    public string UserEmail { get; }
}