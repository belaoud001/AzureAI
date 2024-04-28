using AzureAiAPI.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AzureAiAPI.ModelBinders;

public class MultipartJsonModelBinderProvider : IModelBinderProvider
{
    
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var modelType = context.Metadata.UnderlyingOrModelType;
        if (modelType == typeof(Request))
        {
            return new MultipartJsonModelBinder();
        }

        return null;
    }
    
}