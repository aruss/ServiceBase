using AngleSharp.Parser.Html;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ServiceBase.Extensions;
using ServiceBase.Notification.Email;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ServiceBase.IdentityServer.Public.IntegrationTests
{
    // http://stackoverflow.com/questions/30557521/how-to-access-httpcontext-inside-a-unit-test-in-asp-net-5-mvc-6

    [Collection("Login")]
    public class RegisterTests
    {
        string _returnUrl = "%2Fconnect%2Fauthorize%2Flogin%3Fclient_id%3Dmvc%26redirect_uri%3Dhttp%253A%252F%252Flocalhost%253A3308%252Fsignin-oidc%26response_type%3Dcode%2520id_token%26scope%3Dopenid%2520profile%2520api1%26response_mode%3Dform_post%26nonce%3D636170876883483776.ZGUwYWY2NDctNDJlNy00MTVmLTkwZTYtZjVjMTQ4ZWVlMzAwMWM2OWNhODQtYzZjOS00ZDljLTk3NTktYWE1ZWExMDEwYzk2%26state%3DCfDJ8McEKbBuVCdHkFjjPyy6vSPN5QZvt6xKTHnnKEyNzXwN1YpWo0Mslqn-wBoHhp9vMSjqo3GQGU7emMMhZlgu0BK3G03m2uqLc5vrYBz06tcWr8S4f9oKl2u1S0cAiJEOw13GnuF-EJ0E3by0nUJ3m1MhhnovobqqTEpKMldmLGpaUxPS4YGxSQVgzDzo3XsyHB4KvWlsdnb3InqNoPKnTQ4ljgDOAeKTAMj39Jz1SMauTcfOXHDyCnJdLt7I0v0up1oY5Az9b7xjzk0oBq5P7lADyq88YTEG0EALJG8SgjYi-Ch-0jd26w74LJ5UyQNScc1ZS4n9dMKUHXvuuIWllzNK86la5X-ydnsNZo2a1HsHyPT4NHe6EG2LdVkh6Y-2-A";

        [Fact]
        public async Task GetRegisterAndConfirmValidLocalUser()
        {
            var emailServiceMock = new Mock<IEmailService>();
            HttpClient client = null;

            Func<string, string, object, Task> sendEmailAsync = async (templateName, email, viewData) =>
            {
                // Get email and confirm registration by calling confirmation link
                var dict = viewData.ToDictionary();
                var confirmResponse = await client.GetAsync((string)dict["ConfirmUrl"]);

                await Task.FromResult(0);
            };

            emailServiceMock.Setup(c =>
                c.SendEmailAsync("AccountCreated", "john@localhost", It.IsAny<object>())).Returns(sendEmailAsync);

            var server = ServerHelper.CreateServer((services) =>
            {
                services.AddSingleton<IEmailService>(emailServiceMock.Object);
            });

            client = server.CreateClient();

            // Navigate to register page
            var response = await client.GetAsync("/register?returnUrl=" + _returnUrl);
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            var doc = (new HtmlParser().Parse(html));

            // Post registration form
            var formPostBodyData = new Dictionary<string, string>
            {
                {"Email","john@localhost"},
                {"Password", "john@localhost"},
                {"PasswordConfirm", "john@localhost" },
                {"__RequestVerificationToken", doc.GetAntiForgeryToken() },
                {"ReturnUrl", doc.GetReturnUrl()}
            };

            var postRequest = response.CreatePostRequest("/register", formPostBodyData);
            var postResponse = await client.SendAsync(postRequest);
            // postResponse.EnsureSuccessStatusCode();


        }

    }
}

