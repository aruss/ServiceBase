// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Sms
{
    using System.Threading.Tasks;

    /// <summary>
    /// ISmsService creates messages from templates and view moel and sends
    /// SMS messages.
    /// </summary>
    public interface ISmsService
    {
        /// <summary>
        /// Sends SMS by creating a message from provided template 
        /// </summary>
        /// <param name="templateName">Template name.</param>
        /// <param name="numberTo">The destination phone number. Format with a
        /// '+' and country code e.g., +16175551212 (E.164 format).</param>
        /// <param name="viewData">Instance of the view model.</param>
        Task SendSmsAsync(
            string templateName,
            string numberTo,
            object viewData);

        /// <summary>
        /// Sends SMS by creating a message from provided template 
        /// </summary>
        /// <param name="templateName">Template name.</param>
        /// <param name="numberTo">The destination phone number. Format with a
        /// '+' and country code e.g., +16175551212 (E.164 format).</param>
        /// <param name="numberFrom">The source phone number. Format with a
        /// '+' and country code e.g., +16175551212 (E.164 format).</param>
        /// <param name="viewData">Instance of the view model.</param>
        Task SendSmsAsync(
            string templateName,
            string numberTo,
            string numberFrom,
            object viewData);
    }
}