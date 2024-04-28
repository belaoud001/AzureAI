namespace AzureAiAPI.Entities;

public class Request
{
    
    public string?    EncodedSource { get; set; }
    public string?    Text          { get; set; }
    public bool?      Translate     { get; set; }
    public IFormFile? AudioSource   { get; set; }
    
}
