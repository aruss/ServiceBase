using ServiceBase.IdentityServer.Config;
using ServiceBase.IdentityServer.Extensions;
using ServiceBase.IdentityServer.Models;
using ServiceBase.IdentityServer.Services;
using IdentityModel;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;

namespace ServiceBase.IdentityServer.Public.UI.Login
{
    public class ExternalController : Controller
    {
        private readonly ApplicationOptions _applicationOptions;
        private readonly ILogger<ExternalController> _logger;
        private readonly IUserAccountStore _userAccountStore;
        private readonly IIdentityServerInteractionService _interaction;


        public ExternalController(
            IOptions<ApplicationOptions> applicationOptions,
            ILogger<ExternalController> logger,
            IUserAccountStore userAccountStore,
            IIdentityServerInteractionService interaction)
        {
            _applicationOptions = applicationOptions.Value;
            _logger = logger;
            _userAccountStore = userAccountStore;
            _interaction = interaction;
        }

        [HttpGet("external/{provider}", Name = "External")]
        public IActionResult Index(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, new AuthenticationProperties
            {
                RedirectUri = "external-callback?returnUrl=" + WebUtility.UrlEncode(returnUrl)
            });
        }

        [HttpGet("external-callback")]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var tempUser = await HttpContext.Authentication.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            if (tempUser == null)
            {
                throw new Exception();
            }

            var claims = tempUser.Claims.ToList();

            var subjectClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
            if (subjectClaim == null)
            {
                subjectClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            }

            if (subjectClaim == null)
            {
                throw new Exception("Unknown userid");
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

            var userAccount = await this._userAccountStore.LoadByExternalProviderAsync(provider, subject);
            if (userAccount != null)
            {
                await HttpContext.Authentication.IssueCookie(userAccount, provider, "external");
                await HttpContext.Authentication.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

                if (returnUrl != null && _interaction.IsValidReturnUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
            }
            else
            {
                userAccount = await this._userAccountStore.LoadByEmailWithExternalAsync(email);
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
                    await HttpContext.Authentication.IssueCookie(userAccount, provider, "external");
                    await HttpContext.Authentication.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

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
                        await HttpContext.Authentication.IssueCookie(userAccount, provider, "external");
                        await HttpContext.Authentication.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

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
