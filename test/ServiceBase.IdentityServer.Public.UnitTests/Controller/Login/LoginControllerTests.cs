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

namespace ServiceBase.IdentityServer.UnitTests.Controller.Login
{
    // http://stackoverflow.com/questions/30557521/how-to-access-httpcontext-inside-a-unit-test-in-asp-net-5-mvc-6

    /*[Collection("Login Controller")]
    public class LoginControllerTests
    {
        /// <summary>
        /// RP redirects to STS, the login page with local login and two IdPs should be shown
        ///
        ///   - Active client
        ///   - Client settings restrict the external logins
        ///   - Client settings allow local login
        /// </summary>
        [Fact]
        public async Task LoginPageWithLocalLogin()
        {
            // Arrange
            var options = new Options<ApplicationOptions>(new ApplicationOptions
            {

            });

            var clientId = "mvc";
            var returnUrl = "%2Fconnect%2Fauthorize%2Flogin%3Fclient_id%3Dmvc%26redirect_uri%3Dhttp%253A%252F%252Flocalhost%253A3308%252Fsignin-oidc%26response_type%3Dcode%2520id_token%26scope%3Dopenid%2520profile%2520api1%26response_mode%3Dform_post%26nonce%3D636167224425361654.MWQyYzcwZTktMGRiYy00YTc0LWI3MTgtMThlMmJiYjQxNTVmZDMyMGI5NjQtOGM4ZS00NjAzLTg5OGYtMTM0ZWYwNjliYWZm%26state%3DCfDJ8McEKbBuVCdHkFjjPyy6vSNHOPb1BvOsBsSu_px4X6gkT_fbm1EmIE-fvTt1RTiHQiK6lVOS9S_AUj5qmkP210O08RGfQd9vsQ2gyMF4TTUVd1dVXedJpzIkpF2rIHs2xG9tbCmpvBHjb-Ln2Fl2ISvwBnKmAQGTRTtbJh75x7WJAFk8MOX_o7MtckUDAX7RxDmlDT09NyjlOkaLl8dnRADP6IABMw5hSU2sQk7G35CI8_kcecHxEKsYscIlDuxuArZTIwWPV8VRMw2fv7ueFZ__az3d3GhrK2nbGhM2EIFlSy6uK4-HIfmbWqU2IaOzWUuUPwKghl0frnD2ANRuqEZO4LNHvkZltST6jgNGACYpFvZPumqAIZoGUWrJ29SMeQ";

            var mockLogger = new Mock<ILogger<LoginController>>();
            var mockInteraction = new Mock<IIdentityServerInteractionService>();
            mockInteraction.Setup(c => c.GetAuthorizationContextAsync(returnUrl))
                .Returns(Task.FromResult(new AuthorizationRequest
                {
                    ClientId = clientId
                }));

            var mockUserAccountStore = new Mock<IUserAccountStore>();
            var mockCrypto = new Mock<ICrypto>();
            var mockClientStore = new Mock<IClientStore>();
            mockClientStore.Setup(c => c.FindClientByIdAsync(clientId)).Returns(Task.FromResult(new Client
            {
                ClientId = clientId,
                ClientName = "Console Client Credentials Flow Sample",
                EnableLocalLogin = true,
                Enabled = true,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                IdentityProviderRestrictions = new string[] { "google", "facebook" },
                AllowedScopes = { "api1", "api2" },
            }));

            var mockHttpContext = new Mock<HttpContext>();
            var mockAuthentication = new Mock<AuthenticationManager>();
            mockAuthentication.Setup(c => c.GetAuthenticationSchemes()).Returns(new AuthenticationDescription[]
            {
                new AuthenticationDescription { DisplayName = "Google", AuthenticationScheme = "google" },
                new AuthenticationDescription { DisplayName = "Facebook", AuthenticationScheme = "facebook" },
                new AuthenticationDescription { DisplayName = "Hotmail", AuthenticationScheme = "hotmail" }
            });
            mockHttpContext.SetupGet(c => c.Authentication).Returns(mockAuthentication.Object);

            var controller = new LoginController(
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

            // Act
            var result = await controller.Login(returnUrl);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<LoginViewModel>(viewResult.ViewData.Model);

            Assert.True(model.EnableLocalLogin);
            Assert.Null(model.Email);
            Assert.Null(model.ErrorMessage);
            Assert.Null(model.InfoMessage);
            Assert.Null(model.Password);
            Assert.False(model.RememberLogin);
            Assert.Equal(returnUrl, model.ReturnUrl);
            Assert.Equal(2, model.ExternalProviders.Count());

            var googleIdp = model.ExternalProviders.FirstOrDefault(c => c.AuthenticationScheme.Equals("google"));
            Assert.NotNull(googleIdp);
            Assert.Equal("google", googleIdp.AuthenticationScheme);
            Assert.Equal("Google", googleIdp.DisplayName);

            var facebookIdp = model.ExternalProviders.FirstOrDefault(c => c.AuthenticationScheme.Equals("facebook"));
            Assert.NotNull(facebookIdp);
            Assert.Equal("facebook", facebookIdp.AuthenticationScheme);
            Assert.Equal("Facebook", facebookIdp.DisplayName);
        }

        /// <summary>
        /// RP redircts to STS with IdP option, since after client restrictions only one possible IdP is allowed to use,
        /// STS should automatically redirect to IdP
        /// </summary>
        [Fact]
        public async Task LoginPageWithProvidedValidIdp()
        {
            // Arrange
            var options = new Options<ApplicationOptions>(new ApplicationOptions
            {

            });

            var clientId = "mvc";
            var returnUrl = "%2Fconnect%2Fauthorize%2Flogin%3Fclient_id%3Dmvc%26redirect_uri%3Dhttp%253A%252F%252Flocalhost%253A3308%252Fsignin-oidc%26response_type%3Dcode%2520id_token%26scope%3Dopenid%2520profile%2520api1%26response_mode%3Dform_post%26nonce%3D636167224425361654.MWQyYzcwZTktMGRiYy00YTc0LWI3MTgtMThlMmJiYjQxNTVmZDMyMGI5NjQtOGM4ZS00NjAzLTg5OGYtMTM0ZWYwNjliYWZm%26state%3DCfDJ8McEKbBuVCdHkFjjPyy6vSNHOPb1BvOsBsSu_px4X6gkT_fbm1EmIE-fvTt1RTiHQiK6lVOS9S_AUj5qmkP210O08RGfQd9vsQ2gyMF4TTUVd1dVXedJpzIkpF2rIHs2xG9tbCmpvBHjb-Ln2Fl2ISvwBnKmAQGTRTtbJh75x7WJAFk8MOX_o7MtckUDAX7RxDmlDT09NyjlOkaLl8dnRADP6IABMw5hSU2sQk7G35CI8_kcecHxEKsYscIlDuxuArZTIwWPV8VRMw2fv7ueFZ__az3d3GhrK2nbGhM2EIFlSy6uK4-HIfmbWqU2IaOzWUuUPwKghl0frnD2ANRuqEZO4LNHvkZltST6jgNGACYpFvZPumqAIZoGUWrJ29SMeQ";

            var mockLogger = new Mock<ILogger<LoginController>>();
            var mockInteraction = new Mock<IIdentityServerInteractionService>();
            mockInteraction.Setup(c => c.GetAuthorizationContextAsync(returnUrl)).Returns(
                Task.FromResult(new AuthorizationRequest
                {
                    ClientId = clientId,
                    IdP = "facebook"
                }));

            var mockUserAccountStore = new Mock<IUserAccountStore>();
            var mockCrypto = new Mock<ICrypto>();
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
            var mockAuthentication = new Mock<AuthenticationManager>();
            mockAuthentication.Setup(c => c.GetAuthenticationSchemes()).Returns(new AuthenticationDescription[]
            {
                new AuthenticationDescription { DisplayName = "Google", AuthenticationScheme = "google" },
                new AuthenticationDescription { DisplayName = "Facebook", AuthenticationScheme = "facebook" },
                new AuthenticationDescription { DisplayName = "Hotmail", AuthenticationScheme = "hotmail" }
            });
            mockHttpContext.SetupGet(c => c.Authentication).Returns(mockAuthentication.Object);

            var controller = new LoginController(
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

            // Act
            var result = await controller.Login(returnUrl);

            // Assert
            var viewResult = Assert.IsType<ChallengeResult>(result);

            Assert.Equal("facebook", viewResult.AuthenticationSchemes.FirstOrDefault());
        }

        /// <summary>
        /// RP redircts to STS with IdP option, since after client restrictions only one possible IdP is allowed to use
        /// but don't match the allowed IdPs for that client STS will show a local login, since it is the only option
        /// </summary>
        [Fact]
        public async Task LoginPageWithProvidedInvalidIdp()
        {
            // Arrange
            var options = new Options<ApplicationOptions>(new ApplicationOptions
            {

            });

            var clientId = "mvc";
            var returnUrl = "%2Fconnect%2Fauthorize%2Flogin%3Fclient_id%3Dmvc%26redirect_uri%3Dhttp%253A%252F%252Flocalhost%253A3308%252Fsignin-oidc%26response_type%3Dcode%2520id_token%26scope%3Dopenid%2520profile%2520api1%26response_mode%3Dform_post%26nonce%3D636167224425361654.MWQyYzcwZTktMGRiYy00YTc0LWI3MTgtMThlMmJiYjQxNTVmZDMyMGI5NjQtOGM4ZS00NjAzLTg5OGYtMTM0ZWYwNjliYWZm%26state%3DCfDJ8McEKbBuVCdHkFjjPyy6vSNHOPb1BvOsBsSu_px4X6gkT_fbm1EmIE-fvTt1RTiHQiK6lVOS9S_AUj5qmkP210O08RGfQd9vsQ2gyMF4TTUVd1dVXedJpzIkpF2rIHs2xG9tbCmpvBHjb-Ln2Fl2ISvwBnKmAQGTRTtbJh75x7WJAFk8MOX_o7MtckUDAX7RxDmlDT09NyjlOkaLl8dnRADP6IABMw5hSU2sQk7G35CI8_kcecHxEKsYscIlDuxuArZTIwWPV8VRMw2fv7ueFZ__az3d3GhrK2nbGhM2EIFlSy6uK4-HIfmbWqU2IaOzWUuUPwKghl0frnD2ANRuqEZO4LNHvkZltST6jgNGACYpFvZPumqAIZoGUWrJ29SMeQ";

            var mockLogger = new Mock<ILogger<LoginController>>();
            var mockInteraction = new Mock<IIdentityServerInteractionService>();
            mockInteraction.Setup(c => c.GetAuthorizationContextAsync(returnUrl))
                .Returns(Task.FromResult(new AuthorizationRequest
                {
                    ClientId = clientId,
                    IdP = "yahoo"
                }));

            var mockUserAccountStore = new Mock<IUserAccountStore>();
            var mockCrypto = new Mock<ICrypto>();
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
            var mockAuthentication = new Mock<AuthenticationManager>();
            mockHttpContext.SetupGet(c => c.Authentication).Returns(mockAuthentication.Object);

            var controller = new LoginController(
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

            // Act
            var result = await controller.Login(returnUrl);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<LoginViewModel>(viewResult.ViewData.Model);
        }

        // TODO: RP redirects to STS with invalid IdP option and client has no local login enabled, in this case an error must be shown

        /// <summary>
        /// TBD;
        /// </summary>
        [Fact]
        public async Task LoginPageWithOnlyOneExternalProvider()
        {
            // Arrange
            var options = new Options<ApplicationOptions>(new ApplicationOptions
            {

            });

            var clientId = "mvc";
            var returnUrl = "%2Fconnect%2Fauthorize%2Flogin%3Fclient_id%3Dmvc%26redirect_uri%3Dhttp%253A%252F%252Flocalhost%253A3308%252Fsignin-oidc%26response_type%3Dcode%2520id_token%26scope%3Dopenid%2520profile%2520api1%26response_mode%3Dform_post%26nonce%3D636167224425361654.MWQyYzcwZTktMGRiYy00YTc0LWI3MTgtMThlMmJiYjQxNTVmZDMyMGI5NjQtOGM4ZS00NjAzLTg5OGYtMTM0ZWYwNjliYWZm%26state%3DCfDJ8McEKbBuVCdHkFjjPyy6vSNHOPb1BvOsBsSu_px4X6gkT_fbm1EmIE-fvTt1RTiHQiK6lVOS9S_AUj5qmkP210O08RGfQd9vsQ2gyMF4TTUVd1dVXedJpzIkpF2rIHs2xG9tbCmpvBHjb-Ln2Fl2ISvwBnKmAQGTRTtbJh75x7WJAFk8MOX_o7MtckUDAX7RxDmlDT09NyjlOkaLl8dnRADP6IABMw5hSU2sQk7G35CI8_kcecHxEKsYscIlDuxuArZTIwWPV8VRMw2fv7ueFZ__az3d3GhrK2nbGhM2EIFlSy6uK4-HIfmbWqU2IaOzWUuUPwKghl0frnD2ANRuqEZO4LNHvkZltST6jgNGACYpFvZPumqAIZoGUWrJ29SMeQ";

            var mockLogger = new Mock<ILogger<LoginController>>();
            var mockInteraction = new Mock<IIdentityServerInteractionService>();
            mockInteraction.Setup(c => c.GetAuthorizationContextAsync(returnUrl))
                .Returns(Task.FromResult(new AuthorizationRequest
                {
                    ClientId = clientId
                }));

            var mockUserAccountStore = new Mock<IUserAccountStore>();
            var mockCrypto = new Mock<ICrypto>();
            var mockClientStore = new Mock<IClientStore>();
            mockClientStore.Setup(c => c.FindClientByIdAsync(clientId)).Returns(Task.FromResult(new Client
            {
                ClientId = clientId,
                ClientName = "Console Client Credentials Flow Sample",
                EnableLocalLogin = false,
                Enabled = true,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "api1", "api2" },
            }));

            var mockHttpContext = new Mock<HttpContext>();
            var mockAuthentication = new Mock<AuthenticationManager>();
            mockAuthentication.Setup(c => c.GetAuthenticationSchemes()).Returns(new AuthenticationDescription[]
            {
                new AuthenticationDescription { DisplayName = "Facebook", AuthenticationScheme = "facebook" }
            });
            mockHttpContext.SetupGet(c => c.Authentication).Returns(mockAuthentication.Object);

            var controller = new LoginController(
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

            // Act
            var result = await controller.Login(returnUrl);

            // Assert
            var viewResult = Assert.IsType<ChallengeResult>(result);

            Assert.Equal("facebook", viewResult.AuthenticationSchemes.FirstOrDefault());
        }
    }

    */
}

