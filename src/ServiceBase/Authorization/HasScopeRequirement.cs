﻿// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Authorization
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;

    public class HasScopeRequirement :
        AuthorizationHandler<HasScopeRequirement>,
        IAuthorizationRequirement
    {
        private readonly string issuer;
        private readonly string scope;

        public HasScopeRequirement(string scope, string issuer)
        {
            this.scope = scope;
            this.issuer = issuer;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            HasScopeRequirement requirement)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User
                .HasClaim(c => c.Type == "scope" && c.Issuer == issuer))
            {
                return Task.CompletedTask;
            }

            // Split the scopes string into an array
            var scopes = context.User
                .FindAll(c => c.Type == "scope" && c.Issuer == issuer)
                .Select(s => s.Value);

            // Succeed if the scope array contains the required scope
            if (scopes.Any(s => s == scope))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}