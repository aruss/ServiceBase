﻿// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Email
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Extensions;
    using ServiceBase.Resources;

    public class DefaultEmailService : IEmailService
    {
        internal readonly DefaultEmailServiceOptions _options;
        internal readonly IEmailSender _emailSender;
        internal readonly IResourceStore _resourceStore;
        internal readonly ILogger<DefaultEmailService> _logger;
        internal readonly ITokenizer _tokenizer;

        public DefaultEmailService(
            DefaultEmailServiceOptions options,
            IEmailSender emailSender,
            IResourceStore resourceStore,
            ILogger<DefaultEmailService> logger,
            ITokenizer tokenizer)
        {
            this._options = options;
            this._emailSender = emailSender;
            this._resourceStore = resourceStore;
            this._logger = logger;
            this._tokenizer = tokenizer;
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
            Resource resource = await this._resourceStore
                .GetEmailTemplateAsync(culture.Name, templateName);

            if (resource == null)
            {
                string msg = $"No resource found for \"{templateName}\"";
                this._logger.LogError(msg);
                throw new NullReferenceException(msg);
            }

            using (TextReader reader = new StringReader(resource.Value))
            {
                XmlSerializer serializer =
                    new XmlSerializer(typeof(EmailTemplate));

                return (EmailTemplate)serializer.Deserialize(reader);
            }
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
            string content = await this._tokenizer
                .Tokenize(template, viewData);

            string layout = await this._tokenizer
                .Tokenize(templateLayout, viewData);

            string html = layout.Replace("{Content}", content);

            return html;
        }

        public async Task SendEmailAsync(
            string templateName,
            string email,
            object viewData)
        {
            await this.SendEmailAsync(
                templateName,
                viewData,
                new string[] { email }
            );
        }

        public async Task SendEmailAsync(
            string templateName,
            object model,
            IEnumerable<string> emailTos = null,
            IEnumerable<string> emailCcs = null,
            IEnumerable<string> emailBccs = null)
        {
            CultureInfo culture = CultureInfo.CurrentUICulture;

            EmailMessage message = new EmailMessage
            {
                EmailTos = emailTos,
                EmailCcs = emailCcs,
                EmailBccs = emailBccs
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
                message.Subject = await this._tokenizer
                    .Tokenize(template.Subject, viewData);

                if (!String.IsNullOrWhiteSpace(templateLayout.Html))
                {
                    message.Html = await this
                        .Tokenize(template.Html, templateLayout.Html, viewData);
                }

                if (!String.IsNullOrWhiteSpace(templateLayout.Text))
                {
                    message.Text = await this
                        .Tokenize(template.Text, templateLayout.Text, viewData);
                }
            }

            await this._emailSender.SendEmailAsync(message);
        }
    }
}
