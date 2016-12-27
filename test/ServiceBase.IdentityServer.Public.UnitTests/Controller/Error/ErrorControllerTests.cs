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

    /*[Collection("Error Controller")]
    public class ErrorControllerTests
    {
        [Fact]
        public async Task IndexActionWithoutAnyParameters()
        {
            // Arrange
            var mockInteraction = new Mock<IIdentityServerInteractionService>();
            var controller = new ErrorController(mockInteraction.Object);

            // Act
            var result = await controller.Index(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            //var model = Assert.IsAssignableFrom<LoginViewModel>(viewResult.ViewData.Model);
        }
    }*/
}

