using System.Threading.Tasks;

namespace ServiceBase.Notification.Sms
{
    public interface ISmsService
    {
        Task SendSmsAsync(string templateName, string to, object viewData);
    }
}