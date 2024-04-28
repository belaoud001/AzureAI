using AzureAiAPI.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AzureAiAPI.ModelBinders;

public class MultipartJsonModelBinder : IModelBinder
{
    
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult != ValueProviderResult.None)
        {
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
            return;
        }

        var httpContext = bindingContext.HttpContext;
        var request     = httpContext.Request;

        // Check if the request has multipart/form-data content
        if (request.HasFormContentType)
        {
            var form  = await request.ReadFormAsync();
            var model = new Request
            {
                AudioSource = form.Files.Count > 0 ? form.Files[0] : null
            };

            bindingContext.Result = ModelBindingResult.Success(model);
            return;
        }

        // Check if the request has JSON content
        if (request.HasJsonContentType())
        {
            var model = await request.ReadFromJsonAsync<Request>();
            
            bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
    
}
