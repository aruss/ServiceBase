namespace ServiceBase.Notification.Sms
{
    using System.Threading.Tasks;

    /// <summary>
    /// Sends SMS 
    /// </summary>
    public interface ISmsSender
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberTo">The destination phone number. Format with a
        /// '+' and country code e.g., +16175551212 (E.164 format).</param>
        /// <param name="numberTo">The source phone number. Format with a
        /// '+' and country code e.g., +16175551212 (E.164 format).</param>
        /// <param name="message">The text of the message you want to send,
        /// limited to 1600 characters.</param>
        Task SendSmsAsync(string numberTo, string numberFrom, string message);

        //Task SendSmsAsync(IEnumerable<string> numberTo, string numberFrom, string message);
        //Task SendSmsAsync(IEnumerable<string> numberTo, string message);
    }
}
