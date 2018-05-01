namespace Plugins.WebHost.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("")]
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        [HttpGet]
        public IActionResult Get()
        {
            return this.View("Home");
        }
    }
}
