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
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http.Authentication;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;

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
        private readonly IClientStore _clientStore;

        public LoginController(
            IOptions<ApplicationOptions> applicationOptions,
            ILogger<LoginController> logger,
            IIdentityServerInteractionService interaction,
            IUserAccountStore userAccountStore,
            ICrypto crypto,
            IClientStore clientStore)
        {
            _applicationOptions = applicationOptions.Value;
            _logger = logger;
            _interaction = interaction;
            _userAccountStore = userAccountStore;
            _crypto = crypto;
            _clientStore = clientStore;
        }

        /// <summary>
        /// Show login page
        /// </summary>
        [HttpGet("login", Name = "Login")]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null)
            {
                // If IdP is passed, then bypass showing the login screen
                return ExternalLogin(context.IdP, returnUrl);
            }

            var vm = await BuildLoginViewModelAsync(returnUrl, context);

            if (vm.EnableLocalLogin == false && vm.ExternalProviders.Count() == 1)
            {
                // Only one option for logging in, so redirect to it automatically
                return ExternalLogin(vm.ExternalProviders.First().AuthenticationScheme, returnUrl);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from email (username)/password login
        /// </summary>
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
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
                                    await this.HttpContext.Authentication.IssueCookie(userAccount,
                                         IdentityServerConstants.LocalIdentityProvider, "password");

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
                                    // If yes show them as sign in hints
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

        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl, AuthorizationRequest context)
        {
            var providers = HttpContext.Authentication.GetAuthenticationSchemes()
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.AuthenticationScheme
                });

            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme));
                    }
                }
            }

            return new LoginViewModel
            {
                EnableLocalLogin = allowLocal,
                ReturnUrl = returnUrl,
                Email = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl, context);
            vm.Email = model.Email;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        /// <summary>
        /// Initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            if (returnUrl != null)
            {
                returnUrl = UrlEncoder.Default.Encode(returnUrl);
            }

            returnUrl = "external-callback?returnUrl=" + returnUrl;

            // Start challenge and roundtrip the return URL
            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl,
                Items = { { "scheme", provider } }
            };

            return new ChallengeResult(provider, props);
        }
    }
}
