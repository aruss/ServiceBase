using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Host.Notification.Email
{
    public class SendGridOptions
    {
        public string User { get; set; }
        public string Key { get; set; }
    }



    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridOptions _options;
        private readonly ILogger<SendGridEmailSender> _logger;

        public SendGridEmailSender(
            IOptions<SendGridOptions> options,
            ILogger<SendGridEmailSender> logger)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            /*var sgm = new SendGrid.SendGridMessage();
            sgm.AddTo(message.Email);
            sgm.From = new System.Net.Mail.MailAddress("Joe@contoso.com", "Joe Smith");
            sgm.Subject = message.Subject;
            
            sgm.Text = message.Text;
            sgm.Html = message.Html;

            var credentials = new System.Net.NetworkCredential(_options.User, _options.Key); 
            var transportWeb = new SendGrid.Web(credentials);
            
            if (transportWeb != null)
            {
                await transportWeb.DeliverAsync(sgm);
            }
            else
            {
                await Task.FromResult(0);
            }*/

            //  File.WriteAllText("C:\\temp\\puckup\\" + Guid.NewGuid().ToString() + ".txt", JsonConvert.SerializeObject(message, Formatting.Indented));
            
            _logger.LogInformation(JsonConvert.SerializeObject(message));
            await Task.FromResult(0);
        }
    }
}
