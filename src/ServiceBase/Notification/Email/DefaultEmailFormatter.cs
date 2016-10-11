using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ServiceBase.Notification.Email
{
    public class DefaultEmailFormatterOptions
    {
        public string TemplateDirectoryPath { get; set; }
    }

    public class DefaultEmailFormatter : IEmailFormatter
    {
        private readonly DefaultEmailFormatterOptions _options;
        private readonly ILogger<DefaultEmailFormatter> _logger;

        public DefaultEmailFormatter(
            IOptions<DefaultEmailFormatterOptions> options,
            ILogger<DefaultEmailFormatter> logger)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<EmailMessage> FormatAsync(string name, Dictionary<string, object> viewData)
        {
            var emailMessage = new EmailMessage();

            var body = File.ReadAllText(Path.Combine(_options.TemplateDirectoryPath, $"{name}_Body.txt"));
            var subject = File.ReadAllText(Path.Combine(_options.TemplateDirectoryPath, $"{name}_Subject.txt"));

            emailMessage.Subject = TokenizeText(subject, viewData);
            emailMessage.Text = TokenizeText(body, viewData);

            return await Task.FromResult(emailMessage);
        }

        private string TokenizeText(string template, Dictionary<string, object> viewData)
        {
            var result = template;
            foreach (var item in viewData)
            {
                result = result.Replace($"{{{item.Key}}}", item.Value.ToString());
            }

            return result;
        }
    }    
}
