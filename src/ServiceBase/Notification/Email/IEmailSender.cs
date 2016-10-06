using System.Threading.Tasks;

namespace ServiceBase.Notification.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
