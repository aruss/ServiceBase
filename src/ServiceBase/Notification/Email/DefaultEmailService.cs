using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiceBase.Extensions;

namespace ServiceBase.Notification.Email
{
    public class DefaultEmailService : IEmailService
    {
        private readonly IEmailSender _emailSender;
        private readonly DefaultEmailServiceOptions _options;
        private readonly ILogger<DefaultEmailService> _logger;
        private readonly TextFormatter _textFormatter;

        public DefaultEmailService(
            DefaultEmailServiceOptions options,
            ILogger<DefaultEmailService> logger,
            IEmailSender emailSender)
        {
            _logger = logger;
            _options = options;
            _emailSender = emailSender;
            _textFormatter = new TextFormatter();
        }

        public async Task SendEmailAsync(
            string templateName, string email, object viewData, bool sendHtml)
        {
            IDictionary<string, object> dict = viewData as Dictionary<string, object>;
            if (dict == null)
            {
                dict = viewData.ToDictionary();
            }

            var emailMessage = new EmailMessage();

            emailMessage.EmailTo = email;

            // TODO: implement caching 
            emailMessage.Subject = _textFormatter.Format(
                Path.Combine(_options.TemplateDirectoryPath, $"{templateName}_Subject.txt"),
                dict);

            if (sendHtml)
            {
                // TODO: implement razor parsing 
                emailMessage.Html = _textFormatter.Format(
                   Path.Combine(_options.TemplateDirectoryPath, $"{templateName}_Body.cshtml"),
                   dict);
            }
            else
            {
                emailMessage.Text = _textFormatter.Format(
                    Path.Combine(_options.TemplateDirectoryPath, $"{templateName}_Body.txt"),
                    dict);
            }
            
            await _emailSender.SendEmailAsync(emailMessage);
        }
    }
}
