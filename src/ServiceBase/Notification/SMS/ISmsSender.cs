using System.Threading.Tasks;

namespace ServiceBase.Notification.SMS
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
