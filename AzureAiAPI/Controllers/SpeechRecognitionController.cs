using AzureAiAPI.Entities;
using AzureAiAPI.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
namespace AzureAiAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class SpeechRecognitionController : ControllerBase
{

    private readonly ISpeechRecognitionService _speechRecognitionService;
    private readonly ILogger<SpeechRecognitionController> _logger;
    
    public SpeechRecognitionController() { }

    public SpeechRecognitionController(ISpeechRecognitionService speechRecognitionService, 
                                       ILogger<SpeechRecognitionController> logger)
    {
        _speechRecognitionService = speechRecognitionService;
        _logger = logger;
    }

    [HttpGet]
    [Route("/recognise")]
    public async Task<IActionResult> RecogniseSpeechFromEncodedFile(Request request)
    {
        _logger.LogInformation("Recognition from base64 encoded source has been requested ...");
        
        // ToDo : Extend request ...
        var speechInsights = await _speechRecognitionService.RecogniseSpeechAsync(request.EncodedSource);
        
        return new OkObjectResult(speechInsights);
    }

}
