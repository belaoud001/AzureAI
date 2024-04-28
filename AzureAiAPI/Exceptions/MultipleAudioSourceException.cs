namespace AzureAiAPI.Exceptions;

public class MultipleAudioSourceException : Exception
{
    
    public MultipleAudioSourceException() { }
    
    public MultipleAudioSourceException(string message) : base(message) { }
    
}
