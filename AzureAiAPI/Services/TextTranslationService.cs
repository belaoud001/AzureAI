using System.Text;
using AzureAiAPI.Services.Contracts;
using Newtonsoft.Json;

namespace AzureAiAPI.Services;

public class TextTranslationService : ITextTranslationService
{
    
    private readonly ILogger<TextTranslationService> _logger;
    private readonly string azureKey = Environment.GetEnvironmentVariable("AZURE_AI_KEY");
    private readonly string azureRegion = Environment.GetEnvironmentVariable("AZURE_AI_REGION");
    
    public TextTranslationService() { }

    public TextTranslationService(ILogger<TextTranslationService> logger)
    {
        _logger = logger;
    }

    private string CreateURI(string source, string target)
    {
        string endpoint = "https://api.cognitive.microsofttranslator.com";
        string route = "translate?api-version=3.0&from=" + source + "&to=" + target;

        return endpoint + route;
    }

    public async Task<string> TranslateTextAsync(string source, string target, string text)
    {
        var uri         = CreateURI(source, target);
        object[] body   = { new { Text = text } };
        var requestBody = JsonConvert.SerializeObject(body);

        using (var httpClient = new HttpClient())
        {
            using (var httpRequestMessage = new HttpRequestMessage())
            {
                httpRequestMessage.Method     = HttpMethod.Post;
                httpRequestMessage.RequestUri = new Uri(uri);
                httpRequestMessage.Content    = new StringContent(requestBody, Encoding.UTF8, "application/json");
                httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Key",    azureKey);
                httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Region", azureRegion);

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

                var result = await httpResponseMessage.Content.ReadAsStringAsync();

                return result;
            }
        }
    }
    
}
