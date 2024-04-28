using System.Text.Json.Serialization;
using AzureAiAPI.Enums;

namespace AzureAiAPI.Entities;

public class Request
{
    
    public IFormFile? AudioSource   { get; set; }
    
    public string? EncodedSource { get; set; }
    public string? Text          { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AzureAiOperation? AzureAiOperation { get; set; }
    
}
