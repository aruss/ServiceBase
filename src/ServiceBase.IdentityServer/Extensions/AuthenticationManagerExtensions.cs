using ServiceBase.IdentityServer.Models;
using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Http.Authentication;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Extensions
{
    public static class AuthenticationManagerExtensions
    {
        public static async Task IssueCookieAsync(this AuthenticationManager manager, UserAccount userAccount,
            string identityProvider, string authenticationType, bool isPersistent = false)
        {
            var name = userAccount.Claims != null ? userAccount.Claims.Where(x => x.Type == JwtClaimTypes.Name)
                .Select(x => x.Value).FirstOrDefault() ?? userAccount.Email : userAccount.Email;

            var claims = new Claim[] {
                new Claim(JwtClaimTypes.Subject, userAccount.Id.ToString()),
                new Claim(JwtClaimTypes.Name, name),
                new Claim(JwtClaimTypes.IdentityProvider, identityProvider),
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString()),
            };
            var ci = new ClaimsIdentity(claims, authenticationType, JwtClaimTypes.Name, JwtClaimTypes.Role);
            var cp = new ClaimsPrincipal(ci);

            AuthenticationProperties props = null;
            // Only set explicit expiration here if persistent.
            // otherwise we reply upon expiration configured in cookie middleware.
            if (isPersistent)
            {
                props = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1)
                };
            };

            await manager.SignInAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme, cp, props);
        }
    }
}
