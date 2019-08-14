// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase
{
    using System;

    /// <summary>
    /// Configuration utilities
    /// </summary>
    public static class EnironmentUtils
    {
        public static bool IsDevelopmentEnvironment()
        {
            return "Development".Equals(
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                StringComparison.OrdinalIgnoreCase
            );
        }

        public static string GetConfigRoot()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_CONFIGROOT");
        }

        public static string GetContentRoot()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_CONTENTROOT");
        }
    }
}