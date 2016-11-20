using IdentityModel;
using IdentityServer4;
using ServiceBase.IdentityServer.Config;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;

namespace ServiceBase.IdentityServer.EntityFramework.Configuration
{
    static class UserAccounts
    {
        public static List<UserAccount> Get(ICrypto crypto, ApplicationOptions options)
        {
            var now = DateTime.UtcNow;

            var users = new List<UserAccount>
            {
                new UserAccount
                {
                    Id = Guid.Parse("0c2954d2-4c73-44e3-b0f2-c00403e4adef"),
                    Email = "alice@localhost",
                    PasswordHash  = crypto.HashPassword("alice@localhost", options.PasswordHashingIterationCount),
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsEmailVerified = true,
                    IsLoginAllowed = true,
                    Claims = new List<UserClaim>
                    {
                        new UserClaim(JwtClaimTypes.Name, "Alice Smith"),
                        new UserClaim(JwtClaimTypes.GivenName, "Alice"),
                        new UserClaim(JwtClaimTypes.FamilyName, "Smith"),
                        new UserClaim(JwtClaimTypes.Email, "alice@localhost"),
                        new UserClaim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new UserClaim(JwtClaimTypes.Role, "Admin"),
                        new UserClaim(JwtClaimTypes.Role, "Geek"),
                        new UserClaim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new UserClaim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServerConstants.ClaimValueTypes.Json)
                    }
                },
                new UserAccount
                {
                    Id = Guid.Parse("28575826-68a0-4a1d-9428-674a2eb5db95"),
                    Email = "bob@localhost",
                    PasswordHash  = crypto.HashPassword("bob@localhost", options.PasswordHashingIterationCount),
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsEmailVerified = true,
                    IsLoginAllowed = true,
                    Claims = new List<UserClaim>
                    {
                        new UserClaim(JwtClaimTypes.Name, "Bob Smith"),
                        new UserClaim(JwtClaimTypes.GivenName, "Bob"),
                        new UserClaim(JwtClaimTypes.FamilyName, "Smith"),
                        new UserClaim(JwtClaimTypes.Email, "bob@localhost"),
                        new UserClaim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new UserClaim(JwtClaimTypes.Role, "Developer"),
                        new UserClaim(JwtClaimTypes.Role, "Geek"),
                        new UserClaim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new UserClaim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServerConstants.ClaimValueTypes.Json)
                    }
                }
            };

            users.ForEach(c => c.Claims.ToList().ForEach(s => s.UserId = c.Id));

            return users;
        }
    }
}