using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceBase.IdentityServer.Configuration;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.Extensions;
using ServiceBase.IdentityServer.Models;
using ServiceBase.IdentityServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;


namespace ServiceBase.IdentityServer.Public.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult ChallengeExternalLogin(this Controller controller, string provider, string returnUrl)
        {
            if (returnUrl != null)
            {
                returnUrl = UrlEncoder.Default.Encode(returnUrl);
            }

            returnUrl = "external-callback?returnUrl=" + returnUrl;

            // Start challenge and roundtrip the return URL
            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl,
                Items = { { "scheme", provider } }
            };

            return new ChallengeResult(provider, props);
        }
    }
}
