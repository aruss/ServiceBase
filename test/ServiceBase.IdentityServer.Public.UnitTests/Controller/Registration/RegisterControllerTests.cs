using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceBase.IdentityServer.Config;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.Public.UI.Register;
using ServiceBase.IdentityServer.Services;
using ServiceBase.Notification.Email;
using ServiceBase.Xunit;
using System.Threading.Tasks;
using Xunit;

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
