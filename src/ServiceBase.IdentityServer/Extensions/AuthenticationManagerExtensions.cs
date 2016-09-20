using Host.Models;
using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Http.Authentication;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Host.Extensions
{
    public static class AuthenticationManagerExtensions
    {
        public static async Task IssueCookie(this AuthenticationManager manager, UserAccount userAccount, string idp, string amr)
        {
            var name = userAccount.Claims != null ? userAccount.Claims.Where(x => x.Type == JwtClaimTypes.Name)
                .Select(x => x.Value).FirstOrDefault() ?? userAccount.Email : userAccount.Email;

            var claims = new Claim[] {
                new Claim(JwtClaimTypes.Subject, userAccount.Id.ToString()),
                new Claim(JwtClaimTypes.Name, name),
                new Claim(JwtClaimTypes.IdentityProvider, idp),
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString()),
            };
            var ci = new ClaimsIdentity(claims, amr, JwtClaimTypes.Name, JwtClaimTypes.Role);
            var cp = new ClaimsPrincipal(ci);
            
            await manager.SignInAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme, cp);            
        }
    }
}
