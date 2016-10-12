using Microsoft.AspNetCore.Mvc;
using ServiceBase.IdentityServer.Config;
using ServiceBase.IdentityServer.Public.UI.Register;
using ServiceBase.Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ServiceBase.IdentityServer.Crypto;
using Microsoft.Extensions.Logging;
using IdentityServer4.Services;
using ServiceBase.IdentityServer.Services;
using ServiceBase.Notification.Email;

namespace ServiceBase.IdentityServer.UnitTests.Controller.Registration
{
    [Collection("Register")]
    public class RegisterControllerTests
    {
        [Fact]
        public async Task RegisterNewUser()
        {
            // Arrange
            var options = new Options<ApplicationOptions>(new ApplicationOptions
            {

            });

            var mockLogger = new Mock<ILogger<RegisterController>>();
            var mockInteraction = new Mock<IIdentityServerInteractionService>();
            var mockUserAccountStore = new Mock<IUserAccountStore>();
            var mockCrypto = new Mock<ICrypto>();
            mockCrypto.Setup(c => c.GenerateSalt()).Returns("salt");
            mockCrypto.Setup(c => c.Hash("salt")).Returns("hash");


            var mockEmailService = new Mock<IEmailService>();
            var mockEventService = new Mock<IEventService>();
            
            var controller = new RegisterController(
                options,
                mockLogger.Object,
                mockInteraction.Object,
                mockUserAccountStore.Object,
                mockCrypto.Object,
                mockEmailService.Object,
                mockEventService.Object); 

            // Act
            var result = await controller.Index(new RegisterInputModel
            {
                Email = "jim@localhost",
                Password = "password",
                PasswordConfirm = "password",
                ReturnUrl = "http://localhost/foo/bar"
            });
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<RegisterViewModel>(viewResult.ViewData.Model);

            // do stuff 

        }
    }
}
