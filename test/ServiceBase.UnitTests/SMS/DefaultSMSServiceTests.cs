namespace ServiceBase.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using ServiceBase.Logging;
    using ServiceBase.Notification.Sms;
    using Xunit;

    [Collection("ServiceBase")]
    public class DefaultSmsServiceTests
    {
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
            string numberTo = "0123465798";
            string numberFrom = "987654321";

            Mock<ISmsSender> smsSender = new Mock<ISmsSender>();

            Dictionary<string, object> model = new Dictionary<string, object>
            {
                {  "Name" , "Foo" }
            };

            smsSender
                .Setup(c => c
                    .SendSmsAsync(numberTo, numberFrom, It.IsAny<string>()))
                .Returns(new Func<string, string, string, Task>(
                    (nTo, nFrom, msg) =>
                {
                    Assert.NotNull(nTo);
                    Assert.NotNull(nFrom);
                    Assert.NotNull(msg);

                    Assert.Equal(numberTo, nTo);
                    Assert.Equal(nFrom, nFrom);

                    string subject = $"{templateName} {expectedCulture} Foo";
                    Assert.Equal(subject, msg);

                    return Task.FromResult(0);
                }));

            NullLogger<DefaultSmsService> logger =
                new NullLogger<DefaultSmsService>();

            DefaultSmsServiceOptions options = new DefaultSmsServiceOptions
            {
                DefaultCulture = defaultCulture,
                TemplateDirectoryPath = "../../../SMS/Templates"
            };

            Mock<IHttpContextAccessor> httpContextAccessorMock =
                new Mock<IHttpContextAccessor>();

            DefaultSmsService smsService = new DefaultSmsService(
                options,
                logger,
                smsSender.Object,
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
            await smsService.SendSmsAsync(
                templateName,
                numberTo,
                numberFrom,
                model);

            // Reset UI Culture
            Thread.CurrentThread.CurrentCulture = originalCulture;
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
            CultureInfo.CurrentCulture.ClearCachedData();
            CultureInfo.CurrentUICulture.ClearCachedData();
        }
    }
}