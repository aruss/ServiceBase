﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace ServiceBase.IdentityServer.EntityFramework.Entities
{
    public class ClientIdPRestriction
    {
        public Guid Id { get; set; }
        public string Provider { get; set; }
        public Client Client { get; set; }
    }
}