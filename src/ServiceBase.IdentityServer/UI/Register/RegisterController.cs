using Host.Config;
using Host.Crypto;
using Host.Extensions;
using Host.Models;
using Host.Notification.Email;
using Host.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.UI.Login
{
    public class RegisterController : Controller
    {
        private readonly ApplicationOptions _applicationOptions;
        private readonly ILogger<RegisterController> _logger;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IUserAccountStore _userAccountStore;
        private readonly IEmailSender _emailSender;
        private readonly ICrypto _crypto; 
        private readonly IEmailFormatter _emailFormatter;

        public RegisterController(
            IOptions<ApplicationOptions> applicationOptions,
            ILogger<RegisterController> logger,
            IIdentityServerInteractionService interaction,
            IUserAccountStore userAccountStore,
            IEmailSender emailSender,
            ICrypto crypto,
            IEmailFormatter emailFormatter)
        {
            _applicationOptions = applicationOptions.Value;
            _logger = logger;
            _interaction = interaction;
            _userAccountStore = userAccountStore;
            _emailSender = emailSender;
            _crypto = crypto;
            _emailFormatter = emailFormatter;
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

        static readonly string[] UglyBase64 = { "+", "/", "=" };
        protected virtual string StripUglyBase64(string s)
        {
            if (s == null) return s;
            foreach (var ugly in UglyBase64)
            {
                s = s.Replace(ugly, String.Empty);
            }
            return s;
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
                        PasswordChangedAt = now,
                        FailedLoginCount = 0,
                        IsEmailVerified = false,
                        IsLoginAllowed = _applicationOptions.LoginAfterAccountCreation
                    };

                    // TODO: use synchrounous event bus for sending emails, emit events and so on

                    #region Send email verification message 

                    userAccount.SetVerification(
                        StripUglyBase64(_crypto.Hash(_crypto.GenerateSalt())),
                        VerificationKeyPurpose.ConfirmAccount,
                        model.ReturnUrl,
                        now); 

                    var dictionary = new Dictionary<string, object>
                    {
                        { "Email", userAccount.Email },
                        { "Token", userAccount.VerificationKey },
                    };
                    var mailMessage = await _emailFormatter.FormatAsync("AccountCreatedEvent", userAccount, dictionary);
                    await _emailSender.SendEmailAsync(mailMessage);

                    #endregion 
                    
                    // Save user to data store 
                    userAccount = await _userAccountStore.WriteAsync(userAccount);

                    if (_applicationOptions.LoginAfterAccountCreation)
                    {
                        await HttpContext.Authentication.IssueCookie(userAccount, "idsvr", "password");

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
        
        [HttpGet("register/success", Name = "Success")]
        public async Task<IActionResult> Success(string returnUrl, string provider)
        {
            // select propper mail provider and render it as button 

            return View();
        }

        [HttpGet("register/confirm/{key}", Name = "Confirm")]
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

            userAccount.IsLoginAllowed = true; 
            userAccount.IsEmailVerified = true;
            userAccount.EmailVerifiedAt = DateTime.UtcNow;
            userAccount.ClearVerification(); 

            // Update user account 
            userAccount = await _userAccountStore.UpdateAsync(userAccount);

            // TODO: settings for auto signin after confirmation 
            if (_applicationOptions.LoginAfterAccountConfirmation)
            {
                await HttpContext.Authentication.IssueCookie(userAccount, "idsvr", "password");

                if (returnUrl != null && _interaction.IsValidReturnUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
            }

            return Redirect(Url.Action("login", new { ReturnUrl = returnUrl }));
        }

        [HttpGet("register/cancel/{key}", Name = "Cancel")]
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
