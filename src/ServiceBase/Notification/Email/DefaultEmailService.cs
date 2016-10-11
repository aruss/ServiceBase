using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ServiceBase.Notification.Email
{
    public class DefaultEmailService : IEmailService
    {
        private readonly IEmailSender _emailSender; 
        private readonly DefaultEmailServiceOptions _options;
        private readonly ILogger<DefaultEmailService> _logger;
        private readonly TextFormatter _textFormatter;

        public DefaultEmailService(
            IOptions<DefaultEmailServiceOptions> options,
            ILogger<DefaultEmailService> logger,
            IEmailSender emailSender)
        {
            _logger = logger;
            _options = options.Value;
            _emailSender = emailSender;
            _textFormatter = new TextFormatter(); 
        }

        public async Task SendEmailAsync(string templateName, string email, object viewData)
        {
            IDictionary<string, object> dict = viewData as Dictionary<string, object>;
            if (dict == null)
            {
                dict = viewData.ToDictionary(); 
            }
            
            var emailMessage = new EmailMessage();

            emailMessage.Email = email;            

            emailMessage.Subject = _textFormatter.Format(
                Path.Combine(_options.TemplateDirectoryPath, $"{templateName}_Body.txt"),
                dict);

            emailMessage.Text = _textFormatter.Format(
                Path.Combine(_options.TemplateDirectoryPath, $"{templateName}_Subject.txt"),
                dict);

            await _emailSender.SendEmailAsync(emailMessage);
        }
    }
}
