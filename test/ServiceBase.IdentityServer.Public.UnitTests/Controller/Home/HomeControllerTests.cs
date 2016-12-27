using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServiceBase.IdentityServer.Public.UI.Error;
using ServiceBase.IdentityServer.Public.UI.Home;
using System.Threading.Tasks;
using Xunit;

namespace ServiceBase.IdentityServer.UnitTests.Controller.Login
{
    // http://stackoverflow.com/questions/30557521/how-to-access-httpcontext-inside-a-unit-test-in-asp-net-5-mvc-6

    /*[Collection("Home Controller")]
    public class HomeControllerTests
    {
        [Fact]
        public async Task IndexActionWithoutAnyParameters()
        {
            // Arrange
            var mockInteraction = new Mock<IIdentityServerInteractionService>();
            var controller = new HomeController(mockInteraction.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            //var model = Assert.IsAssignableFrom<LoginViewModel>(viewResult.ViewData.Model);

        }
    }*/
}

