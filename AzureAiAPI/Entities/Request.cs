namespace AzureAiAPI.Entities;

public class Request
{
    
    public string? EncodedSource { get; }
    public string? Text          { get; }
    public bool?   translate     { get; }
    
}
