namespace ServiceBase.Notification.Email
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Extensions;

    public class DefaultEmailService : IEmailService
    {
        private readonly IEmailSender emailSender;
        private readonly DefaultEmailServiceOptions options;
        private readonly ILogger<DefaultEmailService> logger;
        private readonly TextFormatter textFormatter;

        public DefaultEmailService(
            DefaultEmailServiceOptions options,
            ILogger<DefaultEmailService> logger,
            IEmailSender emailSender)
        {
            this.logger = logger;
            this.options = options;
            this.emailSender = emailSender;
            this.textFormatter = new TextFormatter();
        }

        public async Task SendEmailAsync(
            string templateName, string email, object viewData, bool sendHtml)
        {
            IDictionary<string, object> dict =
                viewData as Dictionary<string, object>;

            if (dict == null)
            {
                dict = viewData.ToDictionary();
            }

            var emailMessage = new EmailMessage
            {
                EmailTo = email,

                // TODO: implement caching
                Subject = this.textFormatter.Format(
                    Path.Combine(this.options.TemplateDirectoryPath,
                    $"{templateName}_Subject.txt"),
                    dict
                )
            };

            if (sendHtml)
            {
                // TODO: implement razor parsing
                emailMessage.Html = this.textFormatter.Format(
                   Path.Combine(this.options.TemplateDirectoryPath,
                   $"{templateName}_Body.cshtml"),
                   dict);
            }
            else
            {
                emailMessage.Text = this.textFormatter.Format(
                    Path.Combine(this.options.TemplateDirectoryPath,
                    $"{templateName}_Body.txt"),
                    dict);
            }

            await this.emailSender.SendEmailAsync(emailMessage);
        }
    }
}