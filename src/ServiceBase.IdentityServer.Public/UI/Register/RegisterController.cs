using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceBase.IdentityServer.Config;
using ServiceBase.IdentityServer.Crypto;
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

        public RegisterController(
            IOptions<ApplicationOptions> applicationOptions,
            ILogger<RegisterController> logger,
            IIdentityServerInteractionService interaction,
            IUserAccountStore userAccountStore,
            ICrypto crypto,
            IEmailService emailService,
            IEventService eventService)
        {
            _applicationOptions = applicationOptions.Value;
            _logger = logger;
            _interaction = interaction;
            _userAccountStore = userAccountStore;
            _emailService = emailService;
            _crypto = crypto;
            _eventService = eventService;
        }

        [HttpGet("register", Name = "Register")]
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

        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterInputModel model)
        {
            if (ModelState.IsValid)
            {
                var email = model.Email.ToLower();

                // Check if user with same email exists
                var userAccount = await _userAccountStore.LoadByEmailWithExternalAsync(email);

                // If user dont exists create a new one
                if (userAccount == null)
                {
                    var now = DateTime.UtcNow;

                    // Create new user instance
                    userAccount = new UserAccount
                    {
                        Email = model.Email,
                        PasswordHash = _crypto.HashPassword(model.Password, _applicationOptions.PasswordHashingIterationCount),
                        FailedLoginCount = 0,
                        IsEmailVerified = false,
                        IsLoginAllowed = _applicationOptions.LoginAfterAccountCreation,
                        PasswordChangedAt = now,
                        CreatedAt = now,
                        UpdatedAt = now
                    };

                    // Set verification key
                    userAccount.SetVerification(
                        _crypto.Hash(_crypto.GenerateSalt()).StripUglyBase64(),
                        VerificationKeyPurpose.ConfirmAccount,
                        model.ReturnUrl,
                        now);

                    // Save user to data store
                    await _userAccountStore.WriteAsync(userAccount);

                    // Send email
                    await _emailService.SendEmailAsync("AccountCreated", userAccount.Email, new
                    {
                        // TODO: change to read x-forwared-host
                        ConfirmUrl = String.Format("{0}://{1}/register/confirm/{2}", this.Request.Scheme, this.Request.Host, userAccount.VerificationKey),
                        CancelUrl = String.Format("{0}://{1}/register/cancel/{2}", this.Request.Scheme, this.Request.Host, userAccount.VerificationKey)
                    });

                    // Emit event
                    // _eventService.RaiseAccountCreatedEventAsync()

                    if (_applicationOptions.LoginAfterAccountCreation)
                    {
                        await HttpContext.Authentication.IssueCookie(userAccount,
                            IdentityServerConstants.LocalIdentityProvider, "password");

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
                else if (!userAccount.IsLoginAllowed)
                {
                    if (!userAccount.IsEmailVerified)
                    {
                        ModelState.AddModelError("", "Please confirm your email account");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Your user account has be disabled");
                    }
                }
                else
                {
                    // If user has a password then its a local account
                    if (!userAccount.HasPassword())
                    {
                        ModelState.AddModelError("", "User already exists");
                    }
                    else
                    {

                    }

                    // Return list of external account providers as hint
                    var vm = new RegisterViewModel(model);
                    vm.HintExternalAccounts = userAccount.Accounts.Select(s => s.Provider).ToArray();
                    return View(vm);
                }
                // As if user wants to use other account instead

                // if yes, cancel registration and redirect to login
                // if no ask if he wants to merge accounts

                // if yes, link account
                // if no create user
            }

            return View(new RegisterViewModel(model));
        }

        [HttpGet("register/success", Name = "RegisterSuccess")]
        public async Task<IActionResult> Success(string returnUrl, string provider)
        {
            // TODO: Select propper mail provider and render it as button

            return await Task.FromResult(View(new SuccessViewModel
            {
                ReturnUrl = returnUrl,
                Provider = provider
            }));
        }

        [HttpGet("register/confirm/{key}", Name = "RegisterConfirm")]
        public async Task<IActionResult> Confirm(string key)
        {
            // Load token data from database
            var userAccount = await _userAccountStore.LoadByVerificationKeyAsync(key);

            if (userAccount == null)
            {
                // ERROR
            }

            if (userAccount.VerificationPurpose != (int)VerificationKeyPurpose.ConfirmAccount)
            {
                // ERROR
            }

            // TODO: check if user exists
            // TODO: check if token expired

            var returnUrl = userAccount.VerificationStorage;

            var now = DateTime.UtcNow;
            userAccount.IsLoginAllowed = true;
            userAccount.IsEmailVerified = true;
            userAccount.EmailVerifiedAt = now;
            userAccount.UpdatedAt = now;
            userAccount.ClearVerification();

            // Update user account
            await _userAccountStore.WriteAsync(userAccount);

            // TODO: settings for auto signin after confirmation
            if (_applicationOptions.LoginAfterAccountConfirmation)
            {
                await HttpContext.Authentication.IssueCookie(userAccount,
                    IdentityServerConstants.LocalIdentityProvider, "password");

                if (returnUrl != null && _interaction.IsValidReturnUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
            }

            return Redirect(Url.Action("login", new { ReturnUrl = returnUrl }));
        }

        [HttpGet("register/cancel/{key}", Name = "RegisterCancel")]
        public async Task<IActionResult> Cancel(string key)
        {
            // Load token data from database
            var userAccount = await _userAccountStore.LoadByVerificationKeyAsync(key);

            if (userAccount == null)
            {
                // ERROR
            }

            if (userAccount.VerificationPurpose != (int)VerificationKeyPurpose.ConfirmAccount)
            {
                // ERROR
            }

            if (userAccount.LastLoginAt != null)
            {
                // ERROR
            }

            await _userAccountStore.DeleteByIdAsync(userAccount.Id);

            var returnUrl = userAccount.VerificationStorage;

            return Redirect(Url.Action("login", new { returnUrl = returnUrl }));
        }
    }
}
