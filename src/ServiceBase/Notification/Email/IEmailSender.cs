namespace ServiceBase.Notification.Email
{
    using System.Threading.Tasks;

    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage message);
    }
}