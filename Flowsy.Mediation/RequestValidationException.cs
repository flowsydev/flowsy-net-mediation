using Flowsy.Core;

namespace Flowsy.Mediation;

public class RequestValidationException : RequestException
{
    public RequestValidationException(string message, IDictionary<string, string[]> errors, Exception? innerException = null)
        : base(message, innerException)
    {
        Errors = new Dictionary<string, string[]>(errors.Select(
            kv => 
                new KeyValuePair<string, string[]>(kv.Key.ApplyNamingConvention(NamingConvention.CamelCase), kv.Value)
        ));
    }
    
    public RequestValidationException(string message, string propertyName, IEnumerable<string> propertyErrorMessages, Exception? innerException = null)
        : this(
            message,
            new Dictionary<string, string[]>
            {
                { propertyName, propertyErrorMessages.ToArray() }
            },
            innerException
        )
    {
    }
    
    public RequestValidationException(string propertyName, string propertyErrorMessage, Exception? innerException = null)
        : this(propertyErrorMessage, propertyName, propertyErrorMessage, innerException)
    {
    }

    public RequestValidationException(string message, string propertyName, string propertyErrorMessage, Exception? innerException = null)
        : this(
            message,
            new Dictionary<string, string[]>
            {
                { propertyName, new []{ propertyErrorMessage } }
            },
            innerException
        )
    {
    }

    public IDictionary<string, string[]> Errors { get; }
}