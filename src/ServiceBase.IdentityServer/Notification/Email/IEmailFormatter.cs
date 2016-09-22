using ServiceBase.IdentityServer.Models;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Notification.Email
{
    public interface IEmailFormatter
    {
        Task<EmailMessage> FormatAsync(string name, UserAccount account, Dictionary<string, object> viewData);
    }
    
    public class EmailFormatter : IEmailFormatter
    {
        IHostingEnvironment _environment;

        public EmailFormatter(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<EmailMessage> FormatAsync(string name, UserAccount account, Dictionary<string, object> viewData)
        {
            var emailMessage = new EmailMessage();

            var body = File.ReadAllText(Path.Combine(_environment.ContentRootPath, "Notification", "Email", "Templates", $"{name}_Body.txt"));
            var subject = File.ReadAllText(Path.Combine(_environment.ContentRootPath, "Notification", "Email", "Templates", $"{name}_Subject.txt"));

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
