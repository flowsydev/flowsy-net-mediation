namespace Flowsy.Mediation;

/// <summary>
/// Represents an exception thrown during the processing of some user request.
/// This class is intendend to be inherited to throw custom exceptions (CommandException, QueryException, etc) specific to each use case.
/// </summary>
public abstract class RequestException : Exception
{
    protected RequestException(string message, Exception? innerException = default)
        : base(message, innerException)
    {
    }
}