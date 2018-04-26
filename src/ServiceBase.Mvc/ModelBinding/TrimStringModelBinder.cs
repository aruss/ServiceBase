
namespace ServiceBase.Mvc.ModelBinding
{
    // [ModelBinder(BinderType = typeof(TrimStringModelBinder))]

    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    /// Trims input strings.
    ///
    /// </summary>
    public class TrimStringModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string value = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName)
                .FirstValue;

            if (!String.IsNullOrEmpty(value))
            {
                string trimmed = value.Trim();

                bindingContext.ModelState
                    .SetModelValue(bindingContext.ModelName,
                    trimmed, value);

                bindingContext.Result = ModelBindingResult.Success(trimmed);
            }

            return Task.CompletedTask;
        }
    }
}
