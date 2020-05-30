// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Authorization
{
    using Microsoft.AspNetCore.Authorization;

    public class ScopeAuthorizeAttribute : AuthorizeAttribute
    {
        public string Scope
        {
            get
            {
                return this.Policy;
            }
        }

        public ScopeAuthorizeAttribute(
            string scope,
            // bearer is most of the times if you use scoped authentication
            string schemes = "Bearer") 
            : base(scope)
        {
            this.AuthenticationSchemes = "Bearer"; 
        }
    }
}