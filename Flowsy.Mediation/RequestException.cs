namespace Flowsy.Mediation;

/// <summary>
/// Represents an exception thrown during the processing of some user request. 
/// </summary>
public abstract class RequestException : Exception
{
    protected RequestException(string message, Exception? innerException = default)
        : base(message, innerException)
    {
    }
}