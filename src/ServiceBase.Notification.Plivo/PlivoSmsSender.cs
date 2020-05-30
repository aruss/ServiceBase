﻿// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Plivo
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Notification.Sms;

    /// <summary>
    /// Plivo SMS sender
    /// </summary>
    public class PlivoSmsSender : ISmsSender
    {
        private readonly PlivoOptions options;
        private readonly ILogger<PlivoSmsSender> logger;

        /// <summary>
        /// Creates an instance of PlivoSmsSender
        /// </summary>
        /// <param name="options">Instance of
        /// <see cref="TwilioOptions"/></param>
        /// <param name="logger">Instance of
        /// <see cref="ILogger{TwilioSmsSender}"/></param>
        public PlivoSmsSender(
            PlivoOptions options,
            ILogger<PlivoSmsSender> logger)
        {
            this.logger = logger;
            this.options = options;
        }

        /// <summary>
        /// Sends SMS via Plivo service
        /// <see href="https://www.plivo.com/docs/api/message/"></see>
        /// </summary>
        /// <param name="numberTo">The destination phone number. Format with a
        /// '+' and country code e.g., +16175551212 (E.164 format).</param>
        /// <param name="numberFrom">The source phone number. Format with a
        /// '+' and country code e.g., +16175551212 (E.164 format).</param>
        /// <param name="message">The text of the message you want to send,
        /// limited to 1600 characters.</param>
        /// <returns></returns>
        public async Task SendSmsAsync(
            string numberTo,
            string numberFrom,
            string message)
        {
            this.logger.LogInformation(
                $"Send SMS to {numberTo} from {numberFrom} \"{message}\"");

            if (String.IsNullOrEmpty(numberTo))
            {
                throw new ArgumentNullException(nameof(numberTo));
            }

            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Length > 1600)
            {
                throw new ArgumentException(
                    "The text of the message you want to send, limited to " +
                    "1600 characters.",
                    nameof(message));
            }

            if (String.IsNullOrEmpty(numberFrom))
            {
                numberFrom = this.options.From;
            }

            if (String.IsNullOrEmpty(numberFrom))
            {
                throw new ArgumentNullException(nameof(numberTo),
                    "Invalid phone number from in PlivoOptions");
            }

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(
                        $"{this.options.AuthId}:{this.options.Token}")));

                FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("dst", numberTo),
                    new KeyValuePair<string, string>("src", numberFrom),
                    new KeyValuePair<string, string>("text", message)
                });

                string url = "https://api.plivo.com/v1/Account" +
                    $"{this.options.AuthId}/Message";

                HttpResponseMessage result = await client
                    .PostAsync(url, content)
                    .ConfigureAwait(false);

                if (!result.IsSuccessStatusCode)
                {
                    this.logger.LogError(result);
                }
            }
        }
    }
}