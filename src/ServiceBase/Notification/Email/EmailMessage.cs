namespace ServiceBase.Notification.Email
{
    public class EmailMessage
    {
        public string EmailTo { get; set; }
        public string EmailFrom { get; set; }
        public string Subject { get; set; }
        public string Html { get; set;  }
        public string Text { get; set; }
    }
}
