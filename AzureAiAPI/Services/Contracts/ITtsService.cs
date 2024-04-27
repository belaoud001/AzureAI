namespace AzureAiAPI.Services.Contracts;

public interface ITtsService
{
    public Task<Stream> SynthesizeAudioAsync(string text, string language);
}
