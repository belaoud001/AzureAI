using System.Net;
using AzureAiAPI.Exceptions;
using Newtonsoft.Json;

namespace AzureAiAPI.Middlewares;

public class ExceptionHandlerMiddleware
{

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    
    public ExceptionHandlerMiddleware() { }

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private static Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var httpStatusCode = HttpStatusCode.InternalServerError;
        var message        = "Internal Server Error.";

        switch (exception)
        {
            case SourceNotRecognizedException:
                break;
            case InvalidBase64EncodingException:
                httpStatusCode = HttpStatusCode.BadRequest;
                message = "Invalid base64 encoding.";
                break;
            case MissingFieldException:
                httpStatusCode = HttpStatusCode.BadRequest;
                message = "Required field is missing : Json : 'Text' or 'AzureAiOperation' / Multipart : 'AudioSource' (File).";
                break;
            case MultipleAudioSourceException:
                httpStatusCode = HttpStatusCode.BadRequest;
                message = "Multiple files are not supported, we invite you to send one file.";
                break;
        }

        var result = JsonConvert.SerializeObject(new { error = exception.Message });

        httpContext.Response.StatusCode  = (int) httpStatusCode;
        httpContext.Response.ContentType = "application/json";

        return httpContext.Response.WriteAsync(result);
    }

}