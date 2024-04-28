using System.Text;

using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;

using AzureAiAPI.Helpers;
using AzureAiAPI.Entities;
using AzureAiAPI.Exceptions;
using AzureAiAPI.Services.Contracts;

namespace AzureAiAPI.Services;

public class SpeechRecognitionService : ISpeechRecognitionService
{

    private readonly ILogger<SpeechRecognitionService> _logger;
    private readonly string azureKey = Environment.GetEnvironmentVariable("AZURE_AI_KEY");
    private readonly string azureRegion = Environment.GetEnvironmentVariable("AZURE_AI_REGION");
    
    public SpeechRecognitionService() { }

    public SpeechRecognitionService(ILogger<SpeechRecognitionService> logger)
    {
        _logger = logger;
    }

    private Uri GetSpeechServiceURI()
    {
        return new Uri($"wss://{azureRegion}.stt.speech.microsoft.com/speech/universal/v2");
    }

    private SpeechConfig ConfigureSpeechRecognition()
    {
        var endpoint = GetSpeechServiceURI();
        var config = SpeechConfig.FromEndpoint(endpoint, azureKey);
        
        config.SetProperty(PropertyId.SpeechServiceConnection_LanguageIdMode, "Continuous");

        return config;
    }

    private SpeechTranslationConfig ConfigureSpeechTranslation(string source, string target)
    {
        var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(azureKey, azureRegion);

        speechTranslationConfig.SpeechRecognitionLanguage = source;
        speechTranslationConfig.AddTargetLanguage(target);

        return speechTranslationConfig;
    }

    private AudioInputStream CreateAudioInputStream(string encodedString)
    {
        try
        {
            var bytes = Convert.FromBase64String(encodedString);

            return new PullAudioInputStream(new BinaryAudioStreamReader(new BinaryReader(new MemoryStream(bytes))));
        }
        catch
        {
            throw new InvalidBase64EncodingException();
        }
    }
    
    private AudioInputStream CreateAudioInputStream(Stream stream)
    {
        return new PullAudioInputStream(new BinaryAudioStreamReader(new BinaryReader(stream)));
    }

    private async Task<string> IdentifySpeechLanguageAsync(Stream stream)
    {
        string spokenLanguage = null;
        var speechConfig = ConfigureSpeechRecognition();
        var autoDetectSourceLanguageConfig = AutoDetectSourceLanguageConfig.FromOpenRange();
        var audioInputStream = CreateAudioInputStream(stream);
        var stopRecognition = new TaskCompletionSource<int>();

        using (var audioConfig = AudioConfig.FromStreamInput(audioInputStream))
        {
            using (var speechRecognizer = new SpeechRecognizer(speechConfig, autoDetectSourceLanguageConfig, audioConfig))
            {
                speechRecognizer.Recognized += (sender, e) =>
                {
                    if (e.Result.Reason == ResultReason.RecognizedSpeech)
                    {
                        var autoDetectSourceLanguageResult = AutoDetectSourceLanguageResult.FromResult(e.Result);
                        spokenLanguage = autoDetectSourceLanguageResult.Language;
                        stopRecognition.TrySetResult(0);
                    }
                };

                speechRecognizer.Canceled += (sender, e) =>
                {
                    _logger.LogWarning($"CANCELED ~ Reason : {e.Reason}");

                    if (e.Reason == CancellationReason.Error)
                    {
                        _logger.LogError($"CANCELED ~ ErrorCode    : {e.ErrorCode} ");
                        _logger.LogError($"CANCELED ~ ErrorDetails : {e.ErrorDetails}");
                    }

                    stopRecognition.TrySetResult(0);
                };

                speechRecognizer.SessionStarted += (sender, e) =>
                {
                    _logger.LogInformation("Reconition session has been started ...");
                };

                speechRecognizer.SessionStopped += (sender, e) =>
                {
                    _logger.LogInformation("Recognition session has been stopped ...");
                };

                await speechRecognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                Task.WaitAny(new[] { stopRecognition.Task });

                await speechRecognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            }

            if (spokenLanguage == null)
            {
                throw new SourceNotRecognizedException();
            }

            return spokenLanguage;
        }
    }

    public async Task<SpeechInsights> RecogniseSpeechAsync(Stream stream)
    {
        var translatedText = new StringBuilder();
        var source = await IdentifySpeechLanguageAsync(stream);
        var speechTranslationConfig = ConfigureSpeechTranslation(source, "en-US");
        var audioInputStream = CreateAudioInputStream(stream);
        var stopTranslation = new TaskCompletionSource<int>();
        
        using (var audioConfig = AudioConfig.FromStreamInput(audioInputStream))
        {
            using (var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig))
            {
                translationRecognizer.Recognized += (sender, e) =>
                {
                    if (e.Result.Reason == ResultReason.TranslatedSpeech)
                    {
                        foreach (var translation in e.Result.Translations)
                        {
                            translatedText.Append(translation.Value + " ");
                        }
                    }
                };

                translationRecognizer.Canceled += (sender, e) =>
                {
                    _logger.LogWarning($"CANCELED ~ Reason : {e.Reason}");

                    if (e.Reason == CancellationReason.Error)
                    {
                        _logger.LogError($"CANCELED ~ ErrorCode    : {e.ErrorCode} ");
                        _logger.LogError($"CANCELED ~ ErrorDetails : {e.ErrorDetails}");
                    }

                    stopTranslation.TrySetResult(0);
                };

                translationRecognizer.SessionStarted += (sender, e) =>
                {
                    _logger.LogInformation("Translation session has been started ...");
                };

                translationRecognizer.SessionStopped += (sender, e) =>
                {
                    _logger.LogInformation("Translation session has been stopped ...");
                };

                await translationRecognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                Task.WaitAny(new[] { stopTranslation.Task });

                await translationRecognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);

                return new SpeechInsights(source, translatedText.ToString());
            }
        }
    }
    
}
