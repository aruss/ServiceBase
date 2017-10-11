namespace ServiceBase.Mvc
{
    using System;
    using System.Buffers;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Newtonsoft.Json;
    using ServiceBase.Json;

    public static class FormatterCollectionExtensions
    {
        /// <summary>
        /// Adds custom configured <see cref="JsonOutputFormatter"/>.
        /// </summary>
        /// <param name="outputFormatters"></param>
        public static void AddDefaultJsonOutputFormatter(
            this FormatterCollection<IOutputFormatter> outputFormatters)
        {
            var outputFormatter = outputFormatters
                .FirstOrDefault(c => c is JsonOutputFormatter);

            if (outputFormatter != null)
            {
                outputFormatters.Remove(outputFormatter);
            }
            
            outputFormatters.Add(new JsonOutputFormatter(
                new JsonSerializerSettings().SetupDefaults(),
                ArrayPool<Char>.Shared));
        }
    }
}
