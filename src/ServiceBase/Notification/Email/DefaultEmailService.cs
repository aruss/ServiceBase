// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Email
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Extensions;

    public class DefaultEmailService : IEmailService
    {
        internal readonly IEmailSender _emailSender;
        internal readonly DefaultEmailServiceOptions _options;
        internal readonly ILogger<DefaultEmailService> _logger;
        internal readonly IHttpContextAccessor _httpContextAccessor; 

        private static ConcurrentDictionary<string, EmailTemplate> _templates;

        public DefaultEmailService(
            DefaultEmailServiceOptions options,
            ILogger<DefaultEmailService> logger,
            IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor)
        {
            this._logger = logger;
            this._options = options;
            this._emailSender = emailSender;
            this._httpContextAccessor = httpContextAccessor;

            DefaultEmailService._templates =
                new ConcurrentDictionary<string, EmailTemplate>();
        }

        /// <summary>
        /// Resolves the tempalte file path.
        /// </summary>
        /// <param name="culture">Current UI Culture.</param>
        /// <param name="templateName">Name of the file. File pattern
        /// should be SomeTemplate.de-DE.xml</param>
        /// <returns>File path to template file.</returns>
        public virtual async Task<string> GetTemplatePathAsync(
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
                    $"{templateName}.{culture.Name}.xml"
                )
            );

            if (File.Exists(path))
            {
                return path;
            }

            path = Path.GetFullPath(
                Path.Combine(basePath,
                    $"{templateName}.{this._options.DefaultCulture}.xml"
                )
            );

            if (File.Exists(path))
            {
                return path;
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
        public virtual async Task<EmailTemplate> GetTemplate(
            CultureInfo culture,
            string templateName)
        {
            string path =
                await this.GetTemplatePathAsync(culture, templateName);

            return DefaultEmailService._templates.GetOrAdd(path, (p) =>
            {
                this._logger.LogInformation($"Loading email template: {p}");

                using (StreamReader reader = new StreamReader(p))
                {
                    XmlSerializer serializer =
                        new XmlSerializer(typeof(EmailTemplate));

                    return (EmailTemplate)serializer.Deserialize(reader);
                }
            });
        }

        /// <summary>
        /// Replaces template tokens with viewData
        /// </summary>
        /// <param name="template">String template.</param>
        /// <param name="viewData">Dictionary with view data.</param>
        /// <returns>Parsed template.</returns>
        public virtual async Task<string> Tokenize(
            string template,
            IDictionary<string, object> viewData)
        {
            string result = template;
            foreach (var item in viewData)
            {
                result = result
                    .Replace($"{{{item.Key}}}", item.Value.ToString());
            }

            return result;
        }

        /// <summary>
        /// Parses the templates and uses layout file.
        /// </summary>
        /// <param name="template">Template for content.</param>
        /// <param name="templateLayout">Template for layout.</param>
        /// <param name="viewData">Dictionary with view data.</param>
        /// <returns>Parsed template.</returns>
        public virtual async Task<string> Tokenize(
            string template,
            string templateLayout,
            IDictionary<string, object> viewData)
        {
            string content = await this.Tokenize(template, viewData);
            string layout = await this.Tokenize(templateLayout, viewData);
            string html = layout.Replace("{Content}", content);

            return html; 
        }

        /// <summary>
        /// Creates and sends <see cref="EmailMessage"/>.
        /// </summary>
        /// <param name="templateName">Template name.</param>
        /// <param name="email">Destination email address.</param>
        /// <param name="viewData">View Model.</param>
        /// <param name="sendHtml">Send as HTML.</param>
        public async Task SendEmailAsync(
            string templateName,
            string email,
            object model,
            bool sendHtml)
        {
            CultureInfo culture = CultureInfo.CurrentUICulture;
            
            EmailMessage message = new EmailMessage
            {
                EmailTo = email
            };

            IDictionary<string, object> viewData =
               (model as Dictionary<string, object>) ??
               model?.ToDictionary();

            EmailTemplate template =
                await this.GetTemplate(culture, templateName);

            EmailTemplate templateLayout =
                await this.GetTemplate(culture, "_Layout");
            
            if (viewData != null)
            {
                message.Subject = await this
                    .Tokenize(template.Subject, viewData);

                message.Html = await this
                    .Tokenize(template.Html, templateLayout.Html, viewData);

                message.Text = await this
                    .Tokenize(template.Text, templateLayout.Text, viewData);
            }

            await this._emailSender.SendEmailAsync(message);
        }
    }
}
