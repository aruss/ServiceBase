namespace ServiceBase.Notification.Twilio
{
    public class TwilioOptions
    {
        /// <summary>
        /// Twilio session identity.
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Twilio authorization token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Default from phone number. Format with a '+' and country code
        /// e.g., +16175551212 (E.164 format).
        /// </summary>
        public string From { get; set; }
    }
}
