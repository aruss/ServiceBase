using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceBase.IdentityServer.Configuration;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.Extensions;
using ServiceBase.IdentityServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
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
        private readonly UserAccountService _userAccountService;
        private readonly ClientService _clientService;

        public LoginController(
            IOptions<ApplicationOptions> applicationOptions,
            ILogger<LoginController> logger,
            IIdentityServerInteractionService interaction,
            IUserAccountStore userAccountStore,
            ICrypto crypto,
            IClientStore clientStore,
            UserAccountService userAccountService,
            ClientService clientService)
        {
            _applicationOptions = applicationOptions.Value;
            _logger = logger;
            _interaction = interaction;
            _userAccountStore = userAccountStore;
            _crypto = crypto;
            _userAccountService = userAccountService;
            _clientService = clientService;
        }

        /// <summary>
        /// Show login page
        /// </summary>
        [HttpGet("login", Name = "Login")]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            var client = await _clientService.FindEnabledClientByIdAsync(context.ClientId);
            var providers = await _clientService.GetEnabledProvidersAsync(client);

            if (context?.IdP != null)
            {
                // If IdP is passed, then bypass showing the login screen only if client is allowed to signin with provided idp
                if (providers.Any(c => c.AuthenticationScheme.Equals(
                    context.IdP, StringComparison.OrdinalIgnoreCase)))
                {
                    return this.ExternalLogin(context.IdP, returnUrl);
                }
            }

            var vm = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalProviders = providers,
                EnableLocalLogin = client == null ? false : client.EnableLocalLogin,
                Email = context.LoginHint
            };

            if (vm.EnableLocalLogin == false && vm.ExternalProviders.Count() == 1)
            {
                // Only one option for logging in, so redirect to it automatically
                return this.ExternalLogin(vm.ExternalProviders.First()
                    .AuthenticationScheme, returnUrl);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost("login", Name = "Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                // Get user account by provided email
                var userAccount = await _userAccountStore.LoadByEmailAsync(model.Email);
                if (userAccount != null)
                {
                    if (!userAccount.IsLoginAllowed)
                    {
                        ModelState.AddModelError("", "User account is diactivated");
                    }
                    // If user account has local password use password authentication
                    else if (userAccount.HasPassword())
                    {
                        // User has to follow a link in confirmation mail
                        if (_applicationOptions.RequireLocalAccountVerification && !userAccount.IsEmailVerified)
                        {
                            ModelState.AddModelError("", "Please confirm your email account");
                        }
                        // Match passwords
                        else if (_crypto.VerifyPasswordHash(userAccount.PasswordHash,
                            model.Password, _applicationOptions.PasswordHashingIterationCount))
                        {
                            await this.HttpContext.Authentication.IssueCookieAsync(userAccount,
                                IdentityServerConstants.LocalIdentityProvider,
                                "password", model.RememberLogin);

                            // Make sure the returnUrl is still valid, and if yes - redirect back to authorize endpoint
                            if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                            {
                                return Redirect(model.ReturnUrl);
                            }

                            return Redirect("~/");
                        }
                        // Wrong password
                        else
                        {
                            userAccount.FailedLoginCount++;
                            userAccount.LastFailedLoginAt = DateTime.UtcNow;
                            if (userAccount.FailedLoginCount >= _applicationOptions.AccountLockoutFailedLoginAttempts)
                            {
                                userAccount.IsLoginAllowed = false;
                            }
                        }
                    }
                    // In case the user does not have local password but has associated third party accounts
                    // Show the accounts as login hints
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            // Something went wrong, show form with error
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            var client = await _clientService.FindEnabledClientByIdAsync(context.ClientId);
            var providers = await _clientService.GetEnabledProvidersAsync(client);
            var vm = new LoginViewModel(model)
            {
                ExternalProviders = providers,
                EnableLocalLogin = client.EnableLocalLogin,
            };

            return View(vm);
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
