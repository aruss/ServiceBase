namespace ServiceBase.Notification.Email
{
    using System.Threading.Tasks;

    public interface IEmailService
    {
        Task SendEmailAsync(
            string templateName,
            string email,
            object viewData,
            bool sendHtml);
    }
}