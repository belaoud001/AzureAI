using System.Text.Json.Serialization;
using AzureAiAPI.Enums;

namespace AzureAiAPI.Entities;

public class Request
{
    
    public IFormFile? AudioSource { get; set; }
    
    public string? Text           { get; set; }
    public string? SourceLanguage { get; set; }
    public string? TargetLanguage { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AzureAiOperation? AzureAiOperation { get; set; }
    
}
