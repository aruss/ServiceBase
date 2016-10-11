using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceBase.Notification.Email
{
    public interface IEmailFormatter
    {
        Task<EmailMessage> FormatAsync(string name, Dictionary<string, object> viewData);
    }    
}
