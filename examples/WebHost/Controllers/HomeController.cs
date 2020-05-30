// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace WebHost.Controllers
{
    using System;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using WebHost.Models;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public HomeController(ILogger<HomeController> logger, IDiagnosticContext diagnosticContext)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._diagnosticContext = diagnosticContext ?? throw new ArgumentNullException(nameof(diagnosticContext)); 
        }

        public IActionResult Index()
        {

            this._diagnosticContext.Set("Foo", "BAR");


            return View();  
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
