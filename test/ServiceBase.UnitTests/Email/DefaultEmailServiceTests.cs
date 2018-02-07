namespace ServiceBase.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using ServiceBase.Logging;
    using ServiceBase.Notification.Email;
    using Xunit;

    [Collection("ServiceBase")]
    public class DefaultEmailServiceTests
    {
        // Wirte test files
        public void Serialize()
        {
            XmlSerializer serializer =
                new XmlSerializer(typeof(EmailTemplate));

            var obj = new EmailTemplate
            {
                Subject = "Some subject",
                Html = "<div>Hallo Welt, Content: {Content}</div>",
                Text = "Text: {Content}"
            };

            var ms = new MemoryStream();

            serializer.Serialize(ms, obj);

            var xml = Encoding.ASCII.GetString(ms.ToArray());
        }

        [Theory]
        [InlineData("Template1", "en-US", "en-US", "en-US")]
        [InlineData("Template1", "de-DE", "de-DE", "de-DE")]
        [InlineData("Template1", "ru-RU", "en-US", "en-US")]
        public async Task LoadTemplates(
            string templateName,
            string culture,
            string expectedCulture,
            string defaultCulture)
        {
            var email = "alice@localhost";
            var emailSender = new Mock<IEmailSender>();

            var model = new Dictionary<string, object>
            {
                {  "Name" , "Foo" }
            };

            emailSender
                .Setup(c => c.SendEmailAsync(It.IsAny<EmailMessage>()))
                .Returns(new Func<EmailMessage, Task>(async (emailMessage) =>
                {
                    Assert.NotNull(emailMessage);

                    Assert.Equal(email, emailMessage.EmailTo);

                    string subject = $"Subject {templateName} {expectedCulture} Foo";
                    Assert.Equal(subject, emailMessage.Subject);

                    string html = $"HTML LayoutStart {expectedCulture} Html {templateName} {expectedCulture} Foo HTML LayoutEnd {expectedCulture}";
                    Assert.Equal(html, emailMessage.Html);

                    string text = $"Text LayoutStart {expectedCulture} Text {templateName} {expectedCulture} Foo Text LayoutEnd {expectedCulture}";
                    Assert.Equal(text, emailMessage.Text);
                }));

            var logger = new NullLogger<DefaultEmailService>();

            var options = new DefaultEmailServiceOptions
            {
                DefaultCulture = defaultCulture,
                TemplateDirectoryPath = "../../../Email/Templates"
            };

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>(); 

            var emailService = new DefaultEmailService(
                options,
                logger,
                emailSender.Object,
                httpContextAccessorMock.Object);

            // Set culture 
            CultureInfo originalCulture =
                Thread.CurrentThread.CurrentCulture;

            CultureInfo originalUICulture =
                Thread.CurrentThread.CurrentUICulture;

            Thread.CurrentThread.CurrentCulture =
                new CultureInfo(culture, false);

            Thread.CurrentThread.CurrentUICulture =
                new CultureInfo(culture, false); 

            CultureInfo.CurrentCulture.ClearCachedData();
            CultureInfo.CurrentUICulture.ClearCachedData();

            // Send email 
            await emailService.SendEmailAsync(
                templateName,
                email,
                model,
                true);

            // Reset UI Culture
            Thread.CurrentThread.CurrentCulture = originalCulture;
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
            CultureInfo.CurrentCulture.ClearCachedData();
            CultureInfo.CurrentUICulture.ClearCachedData();
        }
    }
}