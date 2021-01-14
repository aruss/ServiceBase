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
    using ServiceBase.Resources;
    using Xunit;
    using System.Linq;

    //[Collection("ServiceBase")]
    public class DefaultEmailServiceTests
    {
        // Wirte test files
        private void Serialize()
        {
            XmlSerializer serializer =
                new XmlSerializer(typeof(EmailTemplate));

            EmailTemplate obj = new EmailTemplate
            {
                Subject = "Some subject",
                Html = "<div>Hallo Welt, Content: {Content}</div>",
                Text = "Text: {Content}"
            };

            MemoryStream ms = new MemoryStream();

            serializer.Serialize(ms, obj);

            string xml = Encoding.ASCII.GetString(ms.ToArray());
        }

        //[Theory]
        //[InlineData("Template1", "en-US", "en-US", "en-US")]
        //[InlineData("Template1", "de-DE", "de-DE", "de-DE")]
        //[InlineData("Template1", "ru-RU", "en-US", "en-US")]
        public async Task LoadTemplates(
            string templateName,
            string culture,
            string expectedCulture,
            string defaultCulture)
        {
            string email = "alice@localhost";
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();

            Dictionary<string, object> model = new Dictionary<string, object>
            {
                {  "Name" , "Foo" }
            };

            emailSender
                .Setup(c => c.SendEmailAsync(It.IsAny<EmailMessage>()))
                .Returns(new Func<EmailMessage, Task>((emailMessage) =>
                {
                    Assert.NotNull(emailMessage);

                    Assert.Equal(email, emailMessage.EmailTos.ElementAt(0));

                    string subject = $"Subject {templateName} {expectedCulture} Foo";
                    Assert.Equal(subject, emailMessage.Subject);

                    string html = $"HTML LayoutStart {expectedCulture} Html {templateName} {expectedCulture} Foo HTML LayoutEnd {expectedCulture}";
                    Assert.Equal(html, emailMessage.Html);

                    string text = $"Text LayoutStart {expectedCulture} Text {templateName} {expectedCulture} Foo Text LayoutEnd {expectedCulture}";
                    Assert.Equal(text, emailMessage.Text);

                    return Task.FromResult(0);
                }));

            NullLogger<DefaultEmailService> logger =
                new NullLogger<DefaultEmailService>();

            DefaultEmailServiceOptions options = new DefaultEmailServiceOptions
            {
                DefaultCulture = defaultCulture
            };

            Mock<IHttpContextAccessor> httpContextAccessorMock =
                new Mock<IHttpContextAccessor>();


            Mock<IResourceStore> resourceStoreMock = new Mock<IResourceStore>();
            resourceStoreMock
                .Setup(c => c.GetEmailTemplateAsync(
                    It.Is<string>(s => s.Equals(culture)),
                    It.Is<string>(s => s.Equals(templateName))
                ))
                .Returns(new Func<string, string, Task<Resource>>((tplCulture, tplKey) =>
                {
                    return Task.FromResult(new Resource
                    {
                        Culture = tplCulture,
                        Key = tplKey,
                        Value = $"HTML LayoutStart {expectedCulture} Html {templateName} {expectedCulture} Foo HTML LayoutEnd {expectedCulture}"
                    });
                }));



            DefaultEmailService emailService = new DefaultEmailService(
                options,
                emailSender.Object,
                resourceStoreMock.Object,
                logger,
                new DefaultTokenizer());

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
                model,
                new string[] { email }
            );

            // Reset UI Culture
            Thread.CurrentThread.CurrentCulture = originalCulture;
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
            CultureInfo.CurrentCulture.ClearCachedData();
            CultureInfo.CurrentUICulture.ClearCachedData();
        }
    }
}