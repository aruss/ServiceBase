// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Sms
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Extensions;

    public class DefaultSmsService : ISmsService
    {
        private readonly ISmsSender _smsSender;
        private readonly DefaultSmsServiceOptions _options;
        private readonly ILogger<DefaultSmsService> _logger;
        private readonly TextFormatter _textFormatter;

        public DefaultSmsService(
            DefaultSmsServiceOptions options,
            ILogger<DefaultSmsService> logger,
            ISmsSender smsSender)
        {
            _logger = logger;
            _options = options;
            _smsSender = smsSender;
            _textFormatter = new TextFormatter();
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
            object viewData)
        {
            await this.SendSmsAsync(templateName, numberTo, null, viewData); 
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
            object viewData)
        {
            IDictionary<string, object> dict =
                 viewData as Dictionary<string, object>;

            if (dict == null)
            {
                dict = viewData.ToDictionary();
            }

            var message = _textFormatter.Format(
                Path.Combine(
                    _options.TemplateDirectoryPath,
                    $"{templateName}.txt"
                ),
                dict
            );

            await _smsSender.SendSmsAsync(numberTo, numberFrom, message);
        }
    }
}