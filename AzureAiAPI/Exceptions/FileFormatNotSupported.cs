namespace AzureAiAPI.Exceptions;

public class FileFormatNotSupported : Exception
{
    
    public FileFormatNotSupported() { }
    
    public FileFormatNotSupported(string message) : base(message) { }
    
}
