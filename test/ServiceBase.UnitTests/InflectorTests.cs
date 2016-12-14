using Microsoft.AspNetCore.Http;
using Moq;
using ServiceBase.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace ServiceBase.IdentityServer.UnitTests.Events
{
    [Collection("ServiceBase")]
    public class InflectorTests
    {
        [Fact]
        public async Task DasherizeTest()
        {
            "foo_bar_baz".Dasherize().Should().Be("foo-bar-baz");
        }
    }
}
