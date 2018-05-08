// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Sms
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Extensions;

    public class DefaultSmsService : ISmsService
    {
        internal readonly ISmsSender _smsSender;
        internal readonly DefaultSmsServiceOptions _options;
        internal readonly ILogger<DefaultSmsService> _logger;
        internal readonly IHttpContextAccessor _httpContextAccessor;

        private static ConcurrentDictionary<string, string> _templates;

        public DefaultSmsService(
            DefaultSmsServiceOptions options,
            ILogger<DefaultSmsService> logger,
            ISmsSender smsSender,
            IHttpContextAccessor httpContextAccessor)
        {
            this._logger = logger;
            this._options = options;
            this._smsSender = smsSender;
            this._httpContextAccessor = httpContextAccessor;

            DefaultSmsService._templates =
                new ConcurrentDictionary<string, string>();
        }

        /// <summary>
        /// Sends SMS by creating a message from provided template 
        /// </summary>
        /// <param name="templateName">Template name.</param>
        /// <param name="numberTo">The destination phone number. Format with a
        /// '+' and country code e.g., +16175551212 (E.164 format).</param>
        /// <param name="viewData">Instance of the view model.</param>
        public async Task SendSmsAsync(
            string templateName,
            string numberTo,
            object model)
        {
            await this.SendSmsAsync(templateName, numberTo, null, model);
        }

        /// <summary>
        /// Sends SMS by creating a message from provided template 
        /// </summary>
        /// <param name="templateName">Template name.</param>
        /// <param name="numberTo">The destination phone number. Format with a
        /// '+' and country code e.g., +16175551212 (E.164 format).</param>
        /// <param name="numberFrom">The source phone number. Format with a
        /// '+' and country code e.g., +16175551212 (E.164 format).</param>
        /// <param name="viewData">Instance of the view model.</param>
        public async Task SendSmsAsync(
            string templateName,
            string numberTo,
            string numberFrom,
            object model)
        {
            CultureInfo culture = CultureInfo.CurrentUICulture;

            IDictionary<string, object> viewData =
              (model as Dictionary<string, object>) ??
              model?.ToDictionary();

            string template = await this.GetTemplate(culture, templateName);
            string message = await this.Tokenize(template, viewData);

            await this._smsSender.SendSmsAsync(numberTo, numberFrom, message);
        }

        /// <summary>
        /// Resolves the tempalte file path.
        /// </summary>
        /// <param name="culture">Current UI Culture.</param>
        /// <param name="templateName">Name of the file. File pattern
        /// should be SomeTemplate.de-DE.txt</param>
        /// <returns>File path to template file.</returns>
        public virtual Task<string> GetTemplatePathAsync(
            CultureInfo culture,
            string templateName)
        {
            string basePath = this._options.TemplateDirectoryPath;

            if (String.IsNullOrWhiteSpace(basePath))
            {
                throw new NullReferenceException(
                    "TemplateDirectoryPath may not be null");
            }

            string path = Path.GetFullPath(
                Path.Combine(basePath,
                    $"{templateName}.{culture.Name}.txt"
                )
            );

            if (File.Exists(path))
            {
                return Task.FromResult(path);
            }

            path = Path.GetFullPath(
                Path.Combine(basePath,
                    $"{templateName}.{this._options.DefaultCulture}.txt"
                )
            );

            if (File.Exists(path))
            {
                return Task.FromResult(path);
            }

            throw new FileNotFoundException(path);
        }

        /// <summary>
        /// Loads template file from file system
        /// </summary>
        /// <param name="culture">Current UI Culture.</param>
        /// <param name="templateName">Name of the file. File pattern
        /// should be SomeTemplate.de-DE.xml</param>
        /// <returns></returns>
        public virtual async Task<string> GetTemplate(
            CultureInfo culture,
            string templateName)
        {
            string path =
                await this.GetTemplatePathAsync(culture, templateName);

            return DefaultSmsService._templates.GetOrAdd(path, (p) =>
            {
                this._logger.LogInformation($"Loading SMS template: {p}");

                return File.ReadAllText(p);
            });
        }

        /// <summary>
        /// Replaces template tokens with viewData
        /// </summary>
        /// <param name="template">String template.</param>
        /// <param name="viewData">Dictionary with view data.</param>
        /// <returns>Parsed template.</returns>
        public virtual Task<string> Tokenize(
            string template,
            IDictionary<string, object> viewData)
        {
            string result = template;
            foreach (KeyValuePair<string, object> item in viewData)
            {
                result = result
                    .Replace($"{{{item.Key}}}", item.Value.ToString());
            }

            return Task.FromResult(result);
        }
    }
}