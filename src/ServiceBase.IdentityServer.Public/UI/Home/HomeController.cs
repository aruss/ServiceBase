using Microsoft.AspNetCore.Mvc;

namespace ServiceBase.IdentityServer.Public.UI.Home
{
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}