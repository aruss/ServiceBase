using ServiceBase.IdentityServer.Config;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.Extensions;
using ServiceBase.IdentityServer.Services;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Public.UI.Login
{
    // https://github.com/IdentityServer/IdentityServer4.Samples/blob/dev/Quickstarts/5_HybridFlowAuthenticationWithApiAccess/src/QuickstartIdentityServer/Controllers/AccountController.cs
    public class LoginController : Controller
    {
        private readonly ApplicationOptions _applicationOptions;
        private readonly ILogger<LoginController> _logger;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IUserAccountStore _userAccountStore;
        private readonly ICrypto _crypto;

        public LoginController(
            IOptions<ApplicationOptions> applicationOptions,
            ILogger<LoginController> logger,
            IIdentityServerInteractionService interaction,
            IUserAccountStore userAccountStore,
            ICrypto crypto)
        {
            _applicationOptions = applicationOptions.Value;
            _logger = logger;
            _interaction = interaction;
            _userAccountStore = userAccountStore;
            _crypto = crypto;
        }

        [HttpGet("login", Name = "Login")]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = new LoginViewModel();

            if (!String.IsNullOrWhiteSpace(returnUrl))
            {
                var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
                if (context != null)
                {
                    vm.Email = context.LoginHint;
                    vm.ReturnUrl = returnUrl;
                }
            }

            return View(vm);
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginInputModel model)
        {
            var vm = new LoginViewModel(model);
            if (ModelState.IsValid)
            {
                // Get user by email 
                var userAccount = await _userAccountStore.LoadByEmailAsync(model.Email);
                if (userAccount != null)
                {
                    // Check if active 
                    if (userAccount.IsLoginAllowed)
                    {
                        // Does the user has a local account? 
                        if (userAccount.HasPassword())
                        {
                            // Check if email already confirmed 
                            if (userAccount.IsEmailVerified)
                            {
                                // Check if needs two factor auth
                                if (_crypto.VerifyPasswordHash(userAccount.PasswordHash, model.Password,
                                    _applicationOptions.PasswordHashingIterationCount))
                                {
                                    await HttpContext.Authentication.IssueCookie(userAccount, "idsvr", "password");

                                    if (model.ReturnUrl != null && _interaction.IsValidReturnUrl(model.ReturnUrl))
                                    {
                                        return Redirect(model.ReturnUrl);
                                    }

                                    return Redirect("~/");
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Invalid username or password.");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Email is not verified yet.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid username or password.");

                            if (_applicationOptions.DisplayLoginHints)
                            {
                                // Does user has third party accounts ? 
                                if (userAccount.Accounts != null && userAccount.Accounts.Count() > 0)
                                {
                                    // If yes show them as sign in hint 
                                    vm.LoginHints = userAccount.Accounts.Select(s => s.Provider).ToArray();
                                }
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "User is deactivated.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
            }

            return View(vm);
        }
    }
}
