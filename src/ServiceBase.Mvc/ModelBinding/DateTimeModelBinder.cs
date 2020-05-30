// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Mvc
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class DateTimeModelBinder : IModelBinder
    {
        private static readonly string[] DateTimeFormats = {
            "yyyyMMdd'T'HHmmss.FFFFFFFK",
            "yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK",
            "yyyy-MM-dd"
        };

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string value = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName).FirstValue;

            if (bindingContext.ModelType == typeof(DateTime?) &&
                String.IsNullOrEmpty(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            bindingContext.Result = DateTime.TryParseExact(
                value,
                DateTimeFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var result) ?
                    ModelBindingResult.Success(result) :
                    ModelBindingResult.Failed();

            return Task.CompletedTask;
        }
    }
}
