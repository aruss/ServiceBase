using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.Notification.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
