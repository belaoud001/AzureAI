namespace AzureAiAPI.Exceptions;

public class InvalidBase64EncodingException : Exception
{
    
    public InvalidBase64EncodingException() {}
    
    public InvalidBase64EncodingException(string message) : base(message) { }
    
}
