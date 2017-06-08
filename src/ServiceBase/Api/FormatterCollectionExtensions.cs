using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Buffers;
using System.Linq;

namespace ServiceBase.Api
{
    public static class FormatterCollectionExtensions
    {
        public static void ReplaceJsonOutputFormatter(this FormatterCollection<IOutputFormatter> outputFormatters)
        {
            var outputFormatter = outputFormatters.FirstOrDefault(c => c is JsonOutputFormatter);
            if (outputFormatter != null)
            {
                outputFormatters.Remove(outputFormatter);
            }

            outputFormatters.Add(new JsonOutputFormatter(new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            }, ArrayPool<Char>.Shared));
        }
    }
}
