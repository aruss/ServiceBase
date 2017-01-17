namespace ServiceBase.Notification.Smtp
{
    public class SmtpOptions
    {
        public string EmailFrom { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
