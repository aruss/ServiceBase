using ServiceBase.IdentityServer.Configuration;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.Extensions;
using ServiceBase.IdentityServer.Models;
using ServiceBase.IdentityServer.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceBase.Notification.Email;

namespace ServiceBase.IdentityServer.Public.UI.Recover
{
    public class RecoverController : Controller
    {
        private readonly ApplicationOptions _applicationOptions;
        private readonly ILogger<RecoverController> _logger;
        private readonly IUserAccountStore _userAccountStore;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEmailService _emailService;
        private readonly ICrypto _crypto;

        public RecoverController(
            IOptions<ApplicationOptions> applicationOptions,
            ILogger<RecoverController> logger,
            IUserAccountStore userAccountStore,
            IIdentityServerInteractionService interaction,
            IEmailService emailService,
            ICrypto crypto)
        {
            _applicationOptions = applicationOptions.Value;
            _logger = logger;
            _userAccountStore = userAccountStore;
            _interaction = interaction;
            _emailService = emailService;
            _crypto = crypto;
        }

        [HttpGet("recover", Name = "Recover")]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = new RecoverViewModel();

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

        [HttpPost("recover")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RecoverInputModel model)
        {
            if (ModelState.IsValid)
            {
                // Load user by email
                var email = model.Email.ToLower();

                // Check if user with same email exists
                var userAccount = await _userAccountStore.LoadByEmailAsync(email);

                if (userAccount != null)
                {
                    userAccount.VerificationKey = StripUglyBase64(_crypto.Hash(_crypto.GenerateSalt()));
                    userAccount.VerificationPurpose = (int)VerificationKeyPurpose.ResetPassword;
                    userAccount.VerificationKeySentAt = DateTime.UtcNow;
                    // account.VerificationStorage = WebUtility.HtmlDecode(model.ReturnUrl);
                    userAccount.VerificationStorage = model.ReturnUrl;

                    await _userAccountStore.WriteAsync(userAccount);

                    await _emailService.SendEmailAsync(IdentityBaseConstants.EmailTemplates.UserAccountRecover, userAccount.Email, new
                    {
                        // TODO: change to read x-forwared-host or use event context
                        ConfirmUrl = Url.Action("Confirm", "Recover", new { Key = userAccount.VerificationKey }, Request.Scheme),
                        CancelUrl = Url.Action("Cancel", "Recover", new { Key = userAccount.VerificationKey }, Request.Scheme)
                    });

                    // Redirect to success page by preserving the email provider name
                    return Redirect(Url.Action("Success", "Recover", new
                    {
                        returnUrl = model.ReturnUrl,
                        provider = userAccount.Email.Split('@').LastOrDefault()
                    }));
                }
                else
                {
                    ModelState.AddModelError("", "User is deactivated.");
                }
            }

            var vm = new RecoverViewModel(model);
            return View(vm);
        }

        [HttpGet("recover/success", Name = "RecoverSuccess")]
        public async Task<IActionResult> Success(string returnUrl, string provider)
        {
            // select propper mail provider and render it as button

            return View();
        }

        [HttpGet("recover/confirm/{key}", Name = "RecoverConfirm")]
        public async Task<IActionResult> Confirm(string key)
        {
            var userAccount = await _userAccountStore.LoadByVerificationKeyAsync(key);
            if (userAccount == null)
            {
                ModelState.AddModelError("", "Invalid token");
                return View("InvalidToken");
            }

            if (userAccount.VerificationPurpose != (int)VerificationKeyPurpose.ResetPassword)
            {
                ModelState.AddModelError("", "Invalid token");
                return View("InvalidToken");
            }

            var returnUrl = userAccount.VerificationStorage;

            var vm = new RecoverViewModel
            {
                ReturnUrl = returnUrl,
                Email = userAccount.Email
            };

            return View(vm);
        }

        [HttpGet("recover/cancel/{key}", Name = "RecoverCancel")]
        public async Task<IActionResult> Cancel(string key)
        {
            // Load token data from database
            var userAccount = await _userAccountStore.LoadByVerificationKeyAsync(key);

            if (userAccount == null)
            {
                // ERROR
            }

            if (userAccount.VerificationPurpose != (int)VerificationKeyPurpose.ResetPassword)
            {
                // ERROR
            }

            if (userAccount.LastLoginAt != null)
            {
                // ERROR
            }

            return Redirect("~/");
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
    }
}
