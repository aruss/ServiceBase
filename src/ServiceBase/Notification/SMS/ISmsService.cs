namespace ServiceBase.Notification.Sms
{
    using System.Threading.Tasks;

    public interface ISmsService
    {
        Task SendSmsAsync(string templateName, string to, object viewData);
    }
}