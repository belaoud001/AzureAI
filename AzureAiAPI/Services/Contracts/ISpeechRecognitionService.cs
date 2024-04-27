using AzureAiAPI.Entities;

namespace AzureAiAPI.Services.Contracts;

public interface ISpeechRecognitionService
{

    Task<SpeechInsights> RecogniseSpeechAsync(String encodedSource);

}
