using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserAccountStore _userAccountStore;

        public ProfileService(IUserAccountStore userAccountStore)
        {
            _userAccountStore = userAccountStore;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userId = Guid.Parse(context.Subject.FindFirst(JwtClaimTypes.Subject).Value);
            var user = _userAccountStore.LoadByIdAsync(userId).Result;

            // TODO: get claims from db user 
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.EmailVerified, user.IsEmailVerified.ToString().ToLower(), ClaimValueTypes.Boolean)
                
               /* new Claim(JwtClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(JwtClaimTypes.GivenName, user.FirstName),
                new Claim(JwtClaimTypes.FamilyName, user.LastName),*/
            };
            
            context.IssuedClaims = claims;

            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var userId = Guid.Parse(context.Subject.FindFirst(JwtClaimTypes.Subject).Value);
            var user = _userAccountStore.LoadByIdAsync(userId).Result;
            
            context.IsActive = user != null && user.IsLoginAllowed;

            return Task.FromResult(0);
        }
    }
}
