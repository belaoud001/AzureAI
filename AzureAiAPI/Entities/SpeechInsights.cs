namespace AzureAiAPI.Entities;

public class SpeechInsights
{
    
    public string Language { get; }
    public string Text     { get; }
    
    public SpeechInsights() { }

    public SpeechInsights(string language, string text)
    {
        Language = language ?? throw new ArgumentNullException(nameof(language));
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }
    
}
