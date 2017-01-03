using IdentityModel;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceBase.IdentityServer.Configuration;
using ServiceBase.IdentityServer.Extensions;
using ServiceBase.IdentityServer.Models;
using ServiceBase.IdentityServer.Public.Extensions;
using ServiceBase.IdentityServer.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Public.UI.Login
{
    public class ExternalController : Controller
    {
        private readonly ApplicationOptions _applicationOptions;
        private readonly ILogger<ExternalController> _logger;
        private readonly IUserAccountStore _userAccountStore;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly UserAccountService _userAccountService;

        public ExternalController(
            IOptions<ApplicationOptions> applicationOptions,
            ILogger<ExternalController> logger,
            IUserAccountStore userAccountStore,
            IIdentityServerInteractionService interaction,
            UserAccountService userAccountService)
        {
            _applicationOptions = applicationOptions.Value;
            _logger = logger;
            _userAccountStore = userAccountStore;
            _interaction = interaction;
            _userAccountService = userAccountService;
        }

        [HttpGet("external/{provider}", Name = "External")]
        public IActionResult Index(string provider, string returnUrl)
        {
            return this.ChallengeExternalLogin(provider, returnUrl);
        }

        [HttpGet("external-callback")]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var tempUser = await HttpContext.Authentication.AuthenticateAsync(
                IdentityServerConstants.ExternalCookieAuthenticationScheme);

            if (tempUser == null)
            {
                throw new Exception("User may not be null");
            }

            var claims = tempUser.Claims.ToList();

            var subjectClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
            if (subjectClaim == null)
            {
                subjectClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            }

            if (subjectClaim == null)
            {
                throw new Exception("Unknown user account ID");
            }

            var loginContext = await _interaction.GetAuthorizationContextAsync(returnUrl);

            claims.Remove(subjectClaim);

            var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                throw new Exception("Unknown email");
            }

            var provider = subjectClaim.Issuer.ToLowerInvariant();
            var subject = subjectClaim.Value;
            var email = emailClaim.Value.ToLowerInvariant();

            var userAccount = await _userAccountStore.LoadByExternalProviderAsync(provider, subject);
            if (userAccount != null)
            {
                await HttpContext.Authentication.IssueCookieAsync(
                    userAccount,
                    provider,
                    IdentityServerConstants.ExternalCookieAuthenticationScheme);

                await HttpContext.Authentication.SignOutAsync(
                    IdentityServerConstants.ExternalCookieAuthenticationScheme);

                if (returnUrl != null && _interaction.IsValidReturnUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
            }
            else
            {
                userAccount = await _userAccountStore.LoadByEmailWithExternalAsync(email);
                if (userAccount == null)
                {
                    // create new user
                    userAccount = new UserAccount
                    {
                        FailedLoginCount = 0,
                        IsLoginAllowed = true,
                        Email = email,
                        IsEmailVerified = true,
                        PasswordHash = null,
                        Accounts = new ExternalAccount[]
                        {
                            new ExternalAccount
                            {
                                Email = email,
                                Provider = provider,
                                Subject = subject
                            }
                        }
                    };

                    await this._userAccountStore.WriteAsync(userAccount);
                    await HttpContext.Authentication.IssueCookieAsync(
                        userAccount,
                        provider,
                        IdentityServerConstants.ExternalCookieAuthenticationScheme);

                    await HttpContext.Authentication.SignOutAsync(
                        IdentityServerConstants.ExternalCookieAuthenticationScheme);

                    if (returnUrl != null && _interaction.IsValidReturnUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                }
                else
                {
                    if (_applicationOptions.MergeAccountsAutomatically)
                    {
                        // join the accounts
                        var now = DateTime.UtcNow;
                        var externalAccount = new ExternalAccount
                        {
                            UserAccountId = userAccount.Id,
                            Email = email,
                            Provider = provider,
                            Subject = subject,
                            CreatedAt = now,
                            LastLoginAt = now
                        };

                        await _userAccountStore.WriteExternalAccountAsync(externalAccount);
                        await HttpContext.Authentication.IssueCookieAsync(
                            userAccount,
                            provider,
                            IdentityServerConstants.ExternalCookieAuthenticationScheme);

                        await HttpContext.Authentication.SignOutAsync(
                            IdentityServerConstants.ExternalCookieAuthenticationScheme);

                        if (returnUrl != null && _interaction.IsValidReturnUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                    }
                    else
                    {
                        // Ask user of he wants to join accounts or create new user
                        throw new NotImplementedException();
                    }
                }
            }

            return Redirect("~/");
        }
    }
}