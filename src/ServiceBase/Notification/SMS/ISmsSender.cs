using System.Threading.Tasks;

namespace ServiceBase.Notification.Sms
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
