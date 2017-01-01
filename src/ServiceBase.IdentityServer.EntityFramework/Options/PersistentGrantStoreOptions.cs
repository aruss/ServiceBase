// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.IdentityServer.EntityFramework.Options
{
    public class PersistentGrantStoreOptions
    {
        public string DefaultSchema { get; set; } = null;

        public TableConfiguration PersistedGrants { get; set; } = new TableConfiguration("PersistedGrants");
    }
}