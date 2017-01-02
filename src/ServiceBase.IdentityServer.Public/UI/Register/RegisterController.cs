using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceBase.IdentityServer.Configuration;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.Events;
using ServiceBase.IdentityServer.Extensions;
using ServiceBase.IdentityServer.Models;
using ServiceBase.IdentityServer.Services;
using ServiceBase.Notification.Email;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Public.UI.Register
{
    public class RegisterController : Controller
    {
        private readonly ApplicationOptions _applicationOptions;
        private readonly ILogger<RegisterController> _logger;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IUserAccountStore _userAccountStore;
        private readonly ICrypto _crypto;
        private readonly IEmailService _emailService;
        private readonly IEventService _eventService;
        private readonly UserAccountService _userAccountService;

        public RegisterController(
            IOptions<ApplicationOptions> applicationOptions,
            ILogger<RegisterController> logger,
            IIdentityServerInteractionService interaction,
            IUserAccountStore userAccountStore,
            ICrypto crypto,
            IEmailService emailService,
            IEventService eventService,
            UserAccountService userAccountService)
        {
            _applicationOptions = applicationOptions.Value;
            _logger = logger;
            _interaction = interaction;
            _userAccountStore = userAccountStore;
            _emailService = emailService;
            _crypto = crypto;
            _eventService = eventService;
            _userAccountService = userAccountService;
        }

        [HttpGet(IdentityBaseConstants.Routes.Register, Name = "Register")]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = new RegisterViewModel();

            if (!String.IsNullOrWhiteSpace(returnUrl))
            {
                var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
                if (request != null)
                {
                    vm.Email = request.LoginHint;
                    vm.ReturnUrl = returnUrl;
                }
            }

            return View(vm);
        }

        private async Task SendUserAccountCreatedAsync(UserAccount userAccount)
        {
            var args = new { Key = userAccount.VerificationKey };

            await _emailService.SendEmailAsync(
                IdentityBaseConstants.EmailTemplates.UserAccountCreated, userAccount.Email, new
                {
                    // TODO: change to read x-forwared-host or use event context
                    ConfirmUrl = Url.Action("Confirm", "Register", args, Request.Scheme),
                    CancelUrl = Url.Action("Cancel", "Register", args, Request.Scheme)
                }
            );
        }

        [HttpPost(IdentityBaseConstants.Routes.Register)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterInputModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if user with same email exists
                var userAccount = await _userAccountStore.LoadByEmailWithExternalAsync(model.Email);

                // If user dont exists create a new one
                if (userAccount == null)
                {
                    userAccount = await _userAccountService.CreateNewLocalUserAccountAsync(
                        model.Email, model.Password, model.ReturnUrl);

                    // Send confirmation mail
                    if (_applicationOptions.RequireLocalAccountVerification)
                    {
                        await SendUserAccountCreatedAsync(userAccount);
                    }

                    if (_applicationOptions.LoginAfterAccountCreation)
                    {
                        await HttpContext.Authentication.IssueCookieAsync(userAccount,
                            IdentityServerConstants.LocalIdentityProvider,
                            IdentityBaseConstants.AuthenticationTypePassword);

                        if (model.ReturnUrl != null && _interaction.IsValidReturnUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                    }
                    else
                    {
                        // Redirect to success page by preserving the email provider name
                        return Redirect(Url.Action("Success", "Register", new
                        {
                            returnUrl = model.ReturnUrl,
                            provider = userAccount.Email.Split('@').LastOrDefault()
                        }));
                    }
                }
                else if (_applicationOptions.RequireLocalAccountVerification && !userAccount.IsEmailVerified)
                {
                    ModelState.AddModelError("", "Please confirm your email account");
                }
                else if (!userAccount.IsLoginAllowed)
                {
                    ModelState.AddModelError("", "Your user account has be disabled");
                }
                // If user has a password then its a local account
                else if (userAccount.HasPassword())
                {
                    ModelState.AddModelError("", "User already exists");
                }
                // External account with same email
                else
                {
                    if (_applicationOptions.MergeAccountsAutomatically)
                    {
                        var now = DateTime.UtcNow;

                        // Set user account password
                        userAccount.PasswordHash = _crypto.HashPassword(model.Password, _applicationOptions.PasswordHashingIterationCount);

                        // If application settings require account verification a verification token will be generated
                        //SetConfirmationVirificationKey(userAccount, model.ReturnUrl, now);

                        // Send email
                        //await SendConfirmationMail(userAccount);

                        if (_applicationOptions.LoginAfterAccountCreation)
                        {
                            await HttpContext.Authentication.IssueCookieAsync(userAccount,
                                IdentityServerConstants.LocalIdentityProvider,
                                IdentityBaseConstants.AuthenticationTypePassword);

                            if (model.ReturnUrl != null && _interaction.IsValidReturnUrl(model.ReturnUrl))
                            {
                                return Redirect(model.ReturnUrl);
                            }
                        }
                        else
                        {
                            // Redirect to success page by preserving the email provider name
                            return Redirect(Url.Action("Success", "Register", new
                            {
                                returnUrl = model.ReturnUrl,
                                provider = userAccount.Email.Split('@').LastOrDefault()
                            }));
                        }
                    }

                    // Return list of external account providers as hint
                    var vm = new RegisterViewModel(model);
                    vm.HintExternalAccounts = userAccount.Accounts.Select(s => s.Provider).ToArray();
                    return View(vm);
                }
            }

            return View(new RegisterViewModel(model));
        }

        [HttpGet(IdentityBaseConstants.Routes.RegisterSuccess, Name = "RegisterSuccess")]
        public async Task<IActionResult> Success(SuccessInputModel model)
        {
            // TODO: Select propper mail provider and render it as button

            var vm = new SuccessViewModel(model);

            return View(vm);
        }

        [HttpGet("register/confirm/{key}", Name = "RegisterConfirm")]
        public async Task<IActionResult> Confirm(string key)
        {
            var result = await _userAccountService.HandleVerificationKey(key,
                VerificationKeyPurpose.ConfirmAccount);

            if (result.UserAccount == null || !result.PurposeValid || result.TokenExpired)
            {
                ModelState.AddModelError("", "Invalid token");
                return View("InvalidToken");
            }

            var returnUrl = result.UserAccount.VerificationStorage;
            await _userAccountService.SetEmailVerifiedAsync(result.UserAccount);

            // If applicatin settings provided login user after confirmation
            if (_applicationOptions.LoginAfterAccountConfirmation)
            {
                await HttpContext.Authentication.IssueCookieAsync(result.UserAccount,
                    IdentityServerConstants.LocalIdentityProvider,
                    IdentityBaseConstants.AuthenticationTypePassword);

                if (returnUrl != null && _interaction.IsValidReturnUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
            }

            return Redirect(Url.Action("Login", "Login", new { ReturnUrl = returnUrl }));
        }

        [HttpGet("register/cancel/{key}", Name = "RegisterCancel")]
        public async Task<IActionResult> Cancel(string key)
        {
            var result = await _userAccountService.HandleVerificationKey(key,
               VerificationKeyPurpose.ConfirmAccount);

            if (result.UserAccount == null || !result.PurposeValid || result.TokenExpired)
            {
                ModelState.AddModelError("", "Invalid token");
                return View("InvalidToken");
            }

            var returnUrl = result.UserAccount.VerificationStorage;
            await _userAccountStore.DeleteByIdAsync(result.UserAccount.Id);
            return Redirect(Url.Action("Login", "Login", new { returnUrl = returnUrl }));
        }
    }
}