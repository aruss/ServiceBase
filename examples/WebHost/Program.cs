// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace WebHost
{
    using System;
    using System.Diagnostics;
    using ServiceBase;

    public class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine($"Process ID: {Process.GetCurrentProcess().Id}");
            return WebHostWrapper.Start<Startup>(args);
        }
    }
}
