﻿using AzureAiAPI.Entities;
using AzureAiAPI.Exceptions;
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
        var request = httpContext.Request;

        if (request.HasFormContentType)
        {
            var form  = await request.ReadFormAsync();
            
            if (form.Files.Count == 0)
            {
                throw new MissingFieldException();
            }
            else if (form.Files.Count == 1)
            {
                var model = new Request
                {
                    AudioSource = form.Files[0]
                };
                
                bindingContext.Result = ModelBindingResult.Success(model);
            }
            else
            {
                throw new MultipleAudioSourceException();
            }
        }

        if (request.HasJsonContentType())
        {
            var model = await request.ReadFromJsonAsync<Request>();

            if (!string.IsNullOrEmpty(model.Text))
            {
                if (model.AzureAiOperation != null)
                    bindingContext.Result = ModelBindingResult.Success(model);
                else
                    throw new MissingFieldException();
            }
            else
            {
                throw new MissingFieldException();
            }
        }
    }
}
