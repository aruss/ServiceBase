// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Localization
{
    using Microsoft.AspNetCore.Routing.Constraints;

    public class CultureRouteConstraint : RegexRouteConstraint
    {
        public CultureRouteConstraint()
            : base(@"^[a-zA-Z]{2}(\-[a-zA-Z]{2})?$") { }
    }
}

