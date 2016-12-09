using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceBase.IdentityServer.Config;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.Public.UI.Login;
using ServiceBase.IdentityServer.Services;
using ServiceBase.Xunit;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System;
using System.Security.Claims;
using IdentityServer4;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ServiceBase.IdentityServer.UnitTests.Controller.Login
{
    // http://stackoverflow.com/questions/30557521/how-to-access-httpcontext-inside-a-unit-test-in-asp-net-5-mvc-6

    [Collection("Login Controller")]
    public class LoginSubmitControllerTests
    {
        Mock<AuthenticationManager> _mockAuthentication;
        Models.UserAccount _userAccount;
        LoginController _controller;
        string _returnUrl = "%2Fconnect%2Fauthorize%2Flogin%3Fclient_id%3Dmvc%26redirect_uri%3Dhttp%253A%252F%252Flocalhost%253A3308%252Fsignin-oidc%26response_type%3Dcode%2520id_token%26scope%3Dopenid%2520profile%2520api1%26response_mode%3Dform_post%26nonce%3D636167224425361654.MWQyYzcwZTktMGRiYy00YTc0LWI3MTgtMThlMmJiYjQxNTVmZDMyMGI5NjQtOGM4ZS00NjAzLTg5OGYtMTM0ZWYwNjliYWZm%26state%3DCfDJ8McEKbBuVCdHkFjjPyy6vSNHOPb1BvOsBsSu_px4X6gkT_fbm1EmIE-fvTt1RTiHQiK6lVOS9S_AUj5qmkP210O08RGfQd9vsQ2gyMF4TTUVd1dVXedJpzIkpF2rIHs2xG9tbCmpvBHjb-Ln2Fl2ISvwBnKmAQGTRTtbJh75x7WJAFk8MOX_o7MtckUDAX7RxDmlDT09NyjlOkaLl8dnRADP6IABMw5hSU2sQk7G35CI8_kcecHxEKsYscIlDuxuArZTIwWPV8VRMw2fv7ueFZ__az3d3GhrK2nbGhM2EIFlSy6uK4-HIfmbWqU2IaOzWUuUPwKghl0frnD2ANRuqEZO4LNHvkZltST6jgNGACYpFvZPumqAIZoGUWrJ29SMeQ";

        public LoginSubmitControllerTests()
        {
            // Arrange
            var options = new Options<ApplicationOptions>(new ApplicationOptions
            {

            });

            var clientId = "mvc";
            _userAccount = new Models.UserAccount
            {
                Email = "alice@localhost",
                PasswordHash = "supersecret",
                IsLoginAllowed = true,
                IsEmailVerified = true,
            };

            var mockLogger = new Mock<ILogger<LoginController>>();
            var mockInteraction = new Mock<IIdentityServerInteractionService>();
            mockInteraction.Setup(c => c.GetAuthorizationContextAsync(_returnUrl))
                .Returns(Task.FromResult(new AuthorizationRequest
                {
                    ClientId = clientId
                }));

            mockInteraction.Setup(c => c.IsValidReturnUrl(_returnUrl)).Returns(true);

            var mockUserAccountStore = new Mock<IUserAccountStore>();
            mockUserAccountStore.Setup(c => c.LoadByEmailAsync(_userAccount.Email)).Returns(Task.FromResult(_userAccount));

            var mockCrypto = new Mock<ICrypto>();
            mockCrypto.Setup(c => c.VerifyPasswordHash(_userAccount.PasswordHash, "supersecret",
                options.Value.PasswordHashingIterationCount)).Returns(true);

            var mockClientStore = new Mock<IClientStore>();
            mockClientStore.Setup(c => c.FindClientByIdAsync(clientId)).Returns(Task.FromResult(new Client
            {
                ClientId = clientId,
                ClientName = "Console Client Credentials Flow Sample",
                Enabled = true,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                IdentityProviderRestrictions = new string[] { "google", "facebook" },
                AllowedScopes = { "api1", "api2" },
            }));
            var mockHttpContext = new Mock<HttpContext>();
            _mockAuthentication = new Mock<AuthenticationManager>();
            _mockAuthentication.Setup(c => c.GetAuthenticationSchemes()).Returns(new AuthenticationDescription[]
            {
                new AuthenticationDescription { DisplayName = "Google", AuthenticationScheme = "google" },
                new AuthenticationDescription { DisplayName = "Facebook", AuthenticationScheme = "facebook" },
                new AuthenticationDescription { DisplayName = "Hotmail", AuthenticationScheme = "hotmail" }
            });
            mockHttpContext.SetupGet(c => c.Authentication).Returns(_mockAuthentication.Object);

            _controller = new LoginController(
                options,
                mockLogger.Object,
                mockInteraction.Object,
                mockUserAccountStore.Object,
                mockCrypto.Object,
                mockClientStore.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };
        }

        public string ReturnUrl
        {
            get
            {
                return _returnUrl;
            }

            set
            {
                _returnUrl = value;
            }
        }

        /// <summary>
        /// Input
        ///  - Valid password
        ///  - Valid email
        ///  - No remember me
        ///
        /// User
        ///  - Has confirmed the registration mail
        ///  - Is active
        ///  - Has not confirmed SMS verfication
        /// </summary>
        [Fact]
        public async Task SubmitLoginWithValidUserAndValidCredentials()
        {
            // Act
            var result = await _controller.Login(new LoginInputModel
            {
                Email = _userAccount.Email,
                Password = "supersecret",
                ReturnUrl = _returnUrl,
                RememberLogin = false
            });

            // Assert
            var viewResult = Assert.IsType<RedirectResult>(result);
            _mockAuthentication.Verify(c => c.SignInAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme, It.IsAny<ClaimsPrincipal>(), null));
            Assert.Equal(_returnUrl, viewResult.Url);
        }

        /// <summary>
        /// Input
        ///  - Valid password
        ///  - Valid email
        ///  - No remember me
        ///
        /// User
        ///  - Has confirmed the registration mail
        ///  - Is active
        ///  - Has not confirmed SMS verfication
        /// </summary>
        [Fact]
        public async Task SubmitLoginWithValidUserAndValidCredentialsAndRememberMe()
        {
            // Act
            var result = await _controller.Login(new LoginInputModel
            {
                Email = _userAccount.Email,
                Password = "supersecret",
                ReturnUrl = _returnUrl,
                RememberLogin = true
            });

            // Assert
            var viewResult = Assert.IsType<RedirectResult>(result);
            _mockAuthentication.Verify(c => c.SignInAsync(
                IdentityServerConstants.DefaultCookieAuthenticationScheme,
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()), Times.Once);
            Assert.Equal(_returnUrl, viewResult.Url);
        }

        /// <summary>
        /// Input
        ///  - Invalid password
        ///  - Valid email
        ///
        /// User
        ///  - Has confirmed the registration mail
        ///  - Is active
        ///  - Has not confirmed SMS verfication
        /// </summary>
        [Fact]
        public async Task SubmitLoginWithWrongPassword()
        {
            // Act
            var result = await _controller.Login(new LoginInputModel
            {
                Email = _userAccount.Email,
                Password = "wrongpassword",
                ReturnUrl = _returnUrl,
                RememberLogin = true
            });

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal("Invalid username or password.",
                viewResult.ViewData.ModelState[""].Errors[0].ErrorMessage);
        }

        /// <summary>
        /// Input
        ///  - Invalid email
        ///
        /// User
        ///  - Has confirmed the registration mail
        ///  - Is active
        ///  - Has not confirmed SMS verfication
        /// </summary>
        [Fact]
        public async Task SubmitLoginWithWrongEmail()
        {
            // Act
            var result = await _controller.Login(new LoginInputModel
            {
                Email = "nouser@localhost",
                Password = "wrongpassword",
                ReturnUrl = _returnUrl,
                RememberLogin = true
            });

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal("Invalid username or password.",
                viewResult.ViewData.ModelState[""].Errors[0].ErrorMessage);
        }

    }
}

