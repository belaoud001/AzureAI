using AzureAiAPI.Entities;
using AzureAiAPI.Enums;
using AzureAiAPI.Helpers;
using AzureAiAPI.ModelBinders;
using AzureAiAPI.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace AzureAiAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class SpeechRecognitionController : ControllerBase
{

    private readonly ISpeechRecognitionService _speechRecognitionService;
    private readonly ITextTranslationService   _textTranslationService;
    private readonly ITtsService               _ttsService;
    
    private readonly ILogger<SpeechRecognitionController> _logger;
    
    public SpeechRecognitionController(ISpeechRecognitionService speechRecognitionService,
                                       ITextTranslationService textTranslationService,
                                       ITtsService ttsService,
                                       ILogger<SpeechRecognitionController> logger)
    {
        _speechRecognitionService = speechRecognitionService;
        _textTranslationService   = textTranslationService;
        _ttsService               = ttsService;
        _logger                   = logger;
    }

    [HttpPost]
    [Route("/recognise")]
    public async Task<IActionResult> RecogniseSpeechFromEncodedFile([ModelBinder(BinderType = typeof(MultipartJsonModelBinder))] Request request)
    {
        _logger.LogInformation("Incoming request to explore Azure AI Speech services ...");

        if (request.AudioSource != null)
        {
            var audioSource    = await AudioSourceValidator.ExtractMemoryStream(request.AudioSource);
            var speechInsights = await _speechRecognitionService.RecogniseSpeechAsync(audioSource);
            
            return new OkObjectResult(speechInsights);
        }

        if (request.Text != null)
        {
            switch (request.AzureAiOperation)
            {
                case AzureAiOperation.Translation:
                    var translatedText = await _textTranslationService.TranslateTextAsync(request.SourceLanguage, 
                                                                                          request.TargetLanguage, 
                                                                                          request.Text);

                    return new OkObjectResult(translatedText);
                case AzureAiOperation.Synthesis:
                    var stream = await _ttsService.SynthesizeAudioAsync(request.Text, 
                                                                        request.SourceLanguage);
                    return new FileStreamResult(stream, new MediaTypeHeaderValue("audio/mp3"))
                    {
                        FileDownloadName = "synthesizedOutput.mp3"
                    };
            }
        }

        return null;

    }

}
