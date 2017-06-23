using System.Threading.Tasks;

namespace ServiceBase.Notification.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string templateName, string email, object viewData, bool sendHtml);
    }
}