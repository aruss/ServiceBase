using AngleSharp.Parser.Html;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ServiceBase.Extensions;
using ServiceBase.IdentityServer.Config;
using ServiceBase.Notification.Email;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Net;

namespace ServiceBase.IdentityServer.Public.IntegrationTests
{
    // http://stackoverflow.com/questions/30557521/how-to-access-httpcontext-inside-a-unit-test-in-asp-net-5-mvc-6

    [Collection("Login")]
    public class RegisterTests
    {
        string _returnUrl = "%2Fconnect%2Fauthorize%2Flogin%3Fclient_id%3Dmvc%26redirect_uri%3Dhttp%253A%252F%252Flocalhost%253A3308%252Fsignin-oidc%26response_type%3Dcode%2520id_token%26scope%3Dopenid%2520profile%2520api1%26response_mode%3Dform_post%26nonce%3D636170876883483776.ZGUwYWY2NDctNDJlNy00MTVmLTkwZTYtZjVjMTQ4ZWVlMzAwMWM2OWNhODQtYzZjOS00ZDljLTk3NTktYWE1ZWExMDEwYzk2%26state%3DCfDJ8McEKbBuVCdHkFjjPyy6vSPN5QZvt6xKTHnnKEyNzXwN1YpWo0Mslqn-wBoHhp9vMSjqo3GQGU7emMMhZlgu0BK3G03m2uqLc5vrYBz06tcWr8S4f9oKl2u1S0cAiJEOw13GnuF-EJ0E3by0nUJ3m1MhhnovobqqTEpKMldmLGpaUxPS4YGxSQVgzDzo3XsyHB4KvWlsdnb3InqNoPKnTQ4ljgDOAeKTAMj39Jz1SMauTcfOXHDyCnJdLt7I0v0up1oY5Az9b7xjzk0oBq5P7lADyq88YTEG0EALJG8SgjYi-Ch-0jd26w74LJ5UyQNScc1ZS4n9dMKUHXvuuIWllzNK86la5X-ydnsNZo2a1HsHyPT4NHe6EG2LdVkh6Y-2-A";

        [Fact]
        public async Task RegisterWithInvalidPassword()
        {
            var server = ServerHelper.CreateServer((services) =>
            {
                var emailServiceMock = new Mock<IEmailService>();
                services.AddSingleton<IEmailService>(emailServiceMock.Object);

                services.Configure<ApplicationOptions>((option) =>
                {
                    option.LoginAfterAccountCreation = false;
                    option.LoginAfterAccountConfirmation = true;
                    option.RequireLocalAccountVerification = true;
                });
            });

            var client = server.CreateClient();

            // 1. Navigate to register page
            var response = await client.GetAsync("/register?returnUrl=" + _returnUrl);
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            var doc = (new HtmlParser().Parse(html));

            // 2. Post registration form
            var formPostBodyData = new Dictionary<string, string>
            {
                {"Email","john@localhost"},
                {"Password", "john@localhost"},
                {"PasswordConfirm", "one that does not match" },
                {"__RequestVerificationToken", doc.GetAntiForgeryToken() },
                {"ReturnUrl", doc.GetReturnUrl()}
            };

            var postRequest = response.CreatePostRequest("/register", formPostBodyData);
            var postResponse = await client.SendAsync(postRequest);

            // 3. The error should be shown
            postResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            postResponse.Headers.Location.Should().BeNull();
        }

        [Fact]
        public async Task RegisterWithInvalidEmail()
        {
            var server = ServerHelper.CreateServer((services) =>
            {
                var emailServiceMock = new Mock<IEmailService>();
                services.AddSingleton<IEmailService>(emailServiceMock.Object);

                services.Configure<ApplicationOptions>((option) =>
                {
                    option.LoginAfterAccountCreation = false;
                    option.LoginAfterAccountConfirmation = true;
                    option.RequireLocalAccountVerification = true;
                });
            });

            var client = server.CreateClient();

            // 1. Navigate to register page
            var response = await client.GetAsync("/register?returnUrl=" + _returnUrl);
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            var doc = (new HtmlParser().Parse(html));

            // 2. Post registration form
            var formPostBodyData = new Dictionary<string, string>
            {
                {"Email","johnocalhost"},
                {"Password", "password"},
                {"PasswordConfirm", "password" },
                {"__RequestVerificationToken", doc.GetAntiForgeryToken() },
                {"ReturnUrl", doc.GetReturnUrl()}
            };

            var postRequest = response.CreatePostRequest("/register", formPostBodyData);
            var postResponse = await client.SendAsync(postRequest);

            // 3. The error should be shown
            postResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            postResponse.Headers.Location.Should().BeNull();
        }


        [Fact]
        public async Task RegisterValidUser_ConfirmLink_LoginAfterConfirmation()
        {
            var emailServiceMock = new Mock<IEmailService>();
            HttpClient client = null;

            Func<string, string, object, Task> sendEmailAsync = async (templateName, email, viewData) =>
            {
                // 4. Get email and confirm registration by calling confirmation link
                var dict = viewData.ToDictionary();
                var confirmResponse = await client.GetAsync((string)dict["ConfirmUrl"]);

                // 5. After confirmation user should be logged in
                confirmResponse.StatusCode.Should().Be(HttpStatusCode.Found);
                confirmResponse.Headers.Location.ToString().Should().StartWith("/connect/authorize/login");
            };

            emailServiceMock.Setup(c =>
                c.SendEmailAsync("UserAccountCreated", "john@localhost", It.IsAny<object>())).Returns(sendEmailAsync);

            var server = ServerHelper.CreateServer((services) =>
            {
                services.AddSingleton<IEmailService>(emailServiceMock.Object);

                services.Configure<ApplicationOptions>((option) =>
                {
                    option.LoginAfterAccountCreation = false;
                    option.LoginAfterAccountConfirmation = true;
                    option.RequireLocalAccountVerification = true;
                });
            });

            client = server.CreateClient();

            // 1. Navigate to register page
            var response = await client.GetAsync("/register?returnUrl=" + _returnUrl);
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            var doc = (new HtmlParser().Parse(html));

            // 2. Post registration form
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

            // 3. The registration success page should be shown
            postResponse.StatusCode.Should().Be(HttpStatusCode.Found);
            postResponse.Headers.Location.ToString().Should().StartWith("/register/success");
        }

        [Fact]
        public async Task RegisterValidUser_ConfirmLink_RedirectToLoginPageAfterConfirmation()
        {
            var emailServiceMock = new Mock<IEmailService>();
            HttpClient client = null;

            Func<string, string, object, Task> sendEmailAsync = async (templateName, email, viewData) =>
            {
                // 4. Get email and confirm registration by calling confirmation link
                var dict = viewData.ToDictionary();
                var confirmResponse = await client.GetAsync((string)dict["ConfirmUrl"]);

                // 5. After confirmation user should be logged in
                confirmResponse.StatusCode.Should().Be(HttpStatusCode.Found);
                confirmResponse.Headers.Location.ToString().Should().StartWith("/login");
            };

            emailServiceMock.Setup(c =>
                c.SendEmailAsync("UserAccountCreated", "john@localhost", It.IsAny<object>())).Returns(sendEmailAsync);

            var server = ServerHelper.CreateServer((services) =>
            {
                services.AddSingleton<IEmailService>(emailServiceMock.Object);

                services.Configure<ApplicationOptions>((option) =>
                {
                    option.LoginAfterAccountCreation = false;
                    option.LoginAfterAccountConfirmation = false;
                    option.RequireLocalAccountVerification = true;
                });
            });

            client = server.CreateClient();

            // 1. Navigate to register page
            var response = await client.GetAsync("/register?returnUrl=" + _returnUrl);
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            var doc = (new HtmlParser().Parse(html));

            // 2. Post registration form
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

            // 3. The registration success page should be shown
            postResponse.StatusCode.Should().Be(HttpStatusCode.Found);
            postResponse.Headers.Location.ToString().Should().StartWith("/register/success");
        }

        [Fact]
        public async Task RegisterValidUser_LoginAfterRegistration_ConfirmLink()
        {
            var emailServiceMock = new Mock<IEmailService>();
            HttpClient client = null;

            Func<string, string, object, Task> sendEmailAsync = async (templateName, email, viewData) =>
            {
                // 4. Get email and confirm registration by calling confirmation link
                var dict = viewData.ToDictionary();
                var confirmResponse = await client.GetAsync((string)dict["ConfirmUrl"]);

                // 5. After confirmation user should be logged in
                confirmResponse.StatusCode.Should().Be(HttpStatusCode.Found);
                confirmResponse.Headers.Location.ToString().Should().StartWith("/connect/authorize/login");
            };

            emailServiceMock.Setup(c =>
                c.SendEmailAsync("UserAccountCreated", "john@localhost", It.IsAny<object>())).Returns(sendEmailAsync);

            var server = ServerHelper.CreateServer((services) =>
            {
                services.AddSingleton<IEmailService>(emailServiceMock.Object);

                services.Configure<ApplicationOptions>((option) =>
                {
                    option.LoginAfterAccountCreation = true;
                    option.LoginAfterAccountConfirmation = true;
                    option.RequireLocalAccountVerification = true;
                });
            });

            client = server.CreateClient();

            // 1. Navigate to register page
            var response = await client.GetAsync("/register?returnUrl=" + _returnUrl);
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            var doc = (new HtmlParser().Parse(html));

            // 2. Post registration form
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

            // 3. The registration success page should be shown
            postResponse.StatusCode.Should().Be(HttpStatusCode.Found);
            postResponse.Headers.Location.ToString().Should().StartWith("/connect/authorize/login");
        }

        [Fact]
        public async Task RegisterValidUser_LoginAfterRegistration_NoConfirmMail()
        {
            var emailServiceMock = new Mock<IEmailService>();
            HttpClient client = null;

            var server = ServerHelper.CreateServer((services) =>
            {
                services.AddSingleton<IEmailService>(emailServiceMock.Object);

                services.Configure<ApplicationOptions>((option) =>
                {
                    option.LoginAfterAccountCreation = true;
                    option.LoginAfterAccountConfirmation = true;
                    option.RequireLocalAccountVerification = false;
                });
            });

            client = server.CreateClient();

            // 1. Navigate to register page
            var response = await client.GetAsync("/register?returnUrl=" + _returnUrl);
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            var doc = (new HtmlParser().Parse(html));

            // 2. Post registration form
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

            // 3. The registration success page should be shown
            postResponse.StatusCode.Should().Be(HttpStatusCode.Found);
            postResponse.Headers.Location.ToString().Should().StartWith("/connect/authorize/login");

            // There should be no email sent
            emailServiceMock.Verify(c => c.SendEmailAsync("UserAccountCreated", "john@localhost", It.IsAny<object>()), Times.Never());
        }

        [Fact]
        public async Task RegisterValidUser_CancelLink()
        {
            var emailServiceMock = new Mock<IEmailService>();
            HttpClient client = null;

            Func<string, string, object, Task> sendEmailAsync = async (templateName, email, viewData) =>
            {
                // 4. Get email and confirm registration by calling confirmation link
                var dict = viewData.ToDictionary();
                var confirmResponse = await client.GetAsync((string)dict["CancelUrl"]);

                // 5. After confirmation user should be redirected to login page
                confirmResponse.StatusCode.Should().Be(HttpStatusCode.Found);
                confirmResponse.Headers.Location.ToString().Should().StartWith("/login");
            };

            emailServiceMock.Setup(c =>
                c.SendEmailAsync("UserAccountCreated", "john@localhost", It.IsAny<object>())).Returns(sendEmailAsync);

            var server = ServerHelper.CreateServer((services) =>
            {
                services.AddSingleton<IEmailService>(emailServiceMock.Object);

                services.Configure<ApplicationOptions>((option) =>
                {
                    option.LoginAfterAccountCreation = false;
                    option.LoginAfterAccountConfirmation = true;
                    option.RequireLocalAccountVerification = true;
                });
            });

            client = server.CreateClient();

            // 1. Navigate to register page
            var response = await client.GetAsync("/register?returnUrl=" + _returnUrl);
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            var doc = (new HtmlParser().Parse(html));

            // 2. Post registration form
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

            // 3. The registration success page should be shown
            postResponse.StatusCode.Should().Be(HttpStatusCode.Found);
            postResponse.Headers.Location.ToString().Should().StartWith("/register/success");
        }

    }
}

