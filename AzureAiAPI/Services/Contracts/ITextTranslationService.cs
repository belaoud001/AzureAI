namespace AzureAiAPI.Services.Contracts;

public interface ITextTranslationService
{

    public Task<string> TranslateTextAsync(string source, string target, string text);

}
