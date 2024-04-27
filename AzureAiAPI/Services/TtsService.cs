using AzureAiAPI.Services.Contracts;
using Microsoft.CognitiveServices.Speech;

namespace AzureAiAPI.Services;

public class TtsService : ITtsService
{
    
    private readonly ILogger<TtsService> _logger;
    private readonly string azureKey = Environment.GetEnvironmentVariable("AZURE_AI_KEY");
    private readonly string azureRegion = Environment.GetEnvironmentVariable("AZURE_AI_REGION");
    
    public TtsService() { }

    public TtsService(ILogger<TtsService> logger)
    {
        _logger = logger;
    }

    private SpeechConfig ConfigureSpeechSynthesis(string language)
    {
        var speechConfig = SpeechConfig.FromSubscription(azureKey, azureRegion);

        speechConfig.SpeechSynthesisLanguage = language;
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz128KBitRateMonoMp3);

        return speechConfig;
    }

    private Stream ConvertAudioDataStreamToStream(AudioDataStream audioDataStream)
    {
        var memoryStream = new MemoryStream();
        // Buffer size of 4096 bytes.
        byte[] buffer = new byte[4 * 1024];
        uint bytesRead;

        while ((bytesRead = audioDataStream.ReadData(buffer)) > 0)
        {
            memoryStream.Write(buffer, 0, (int) bytesRead);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }
    
    public async Task<Stream> SynthesizeAudioAsync(string text, string language)
    {
        var speechConfig = ConfigureSpeechSynthesis(language);

        using (var speechSynthesizer = new SpeechSynthesizer(speechConfig, null))
        {
            var result          = await speechSynthesizer.SpeakTextAsync(text);
            var audioDataStream = AudioDataStream.FromResult(result);
            var memoryStream    = ConvertAudioDataStreamToStream(audioDataStream);

            return memoryStream;
        }
    }
    
}
