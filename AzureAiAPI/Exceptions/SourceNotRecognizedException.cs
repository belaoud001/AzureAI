namespace AzureAiAPI.Exceptions;

public class SourceNotRecognizedException : Exception
{
    
    public SourceNotRecognizedException() { }
    
    public SourceNotRecognizedException(string message) : base(message) { }
    
}
