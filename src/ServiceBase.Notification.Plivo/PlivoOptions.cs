// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Plivo
{
    public class PlivoOptions
    {
        /// <summary>
        /// Plivo authentication identity.
        /// </summary>
        public string AuthId { get; set; }

        /// <summary>
        /// Plivo authorization token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Default from phone number. Format with a '+' and country code
        /// e.g., +16175551212 (E.164 format).
        /// </summary>
        public string From { get; set; }
    }
}