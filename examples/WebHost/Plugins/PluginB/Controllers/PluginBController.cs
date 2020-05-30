// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace PluginB.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Serilog;

    public class PluginBController : Controller
    {
        private readonly ILogger<PluginBController> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PluginBController(
            ILogger<PluginBController> logger,
            IDiagnosticContext diagnosticContext)
        {
            this._logger = logger ??
                throw new ArgumentNullException(nameof(logger));

            this._diagnosticContext = diagnosticContext ??
                throw new ArgumentNullException(nameof(diagnosticContext)); 
        }

        public IActionResult Index()
        {
            return this.Json(new
            {
                Foo = "This crap is coming from plugin b"
            }); 
        }
    }
}
