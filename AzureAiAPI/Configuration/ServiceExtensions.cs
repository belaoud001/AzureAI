using AzureAiAPI.Services;
using AzureAiAPI.Services.Contracts;

namespace AzureAiAPI.Configuration;

public static class ServiceExtensions
{
    
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(
            options => options.AddPolicy(
                "CorsPolicy",
                builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader())
        );
        
        services.AddScoped<ISpeechRecognitionService, SpeechRecognitionService>();
        services.AddScoped<ITextTranslationService, TextTranslationService>();
        services.AddScoped<ITtsService, TtsService>();
    }
    
}
