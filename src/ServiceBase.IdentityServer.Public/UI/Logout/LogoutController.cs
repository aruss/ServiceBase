using IdentityModel;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Public.UI.Logout
{
    public class LogoutController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;

        public LogoutController(IIdentityServerInteractionService interaction)
        {
            _interaction = interaction;
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet("logout", Name = "Logout")]
        public async Task<IActionResult> Logout(string logoutId)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                // if the user is not authenticated, then just show logged out page
                return await Logout(new LogoutViewModel { LogoutId = logoutId });
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                return await Logout(new LogoutViewModel { LogoutId = logoutId });
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            var vm = new LogoutViewModel
            {
                LogoutId = logoutId
            };

            return View(vm);
        }


        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost("logout", Name = "LoggedOut")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            var idp = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                if (model.LogoutId == null)
                {
                    // if there's no current logout context, we need to create one
                    // this captures necessary info from the current logged in user
                    // before we signout and redirect away to the external IdP for signout
                    model.LogoutId = await _interaction.CreateLogoutContextAsync();
                }

                string url = "/Account/Logout?logoutId=" + model.LogoutId;
                try
                {
                    // hack: try/catch to handle social providers that throw
                    await HttpContext.Authentication.SignOutAsync(idp,
                        new AuthenticationProperties { RedirectUri = url });
                }
                catch (NotSupportedException)
                {
                }
            }

            // delete authentication cookie
            await HttpContext.Authentication.SignOutAsync();

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(model.LogoutId);

            var vm = new LoggedOutViewModel
            {
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                SignOutIframeUrl = logout?.SignOutIFrameUrl
            };

            return View("LoggedOut", vm);
        }
    }
}
