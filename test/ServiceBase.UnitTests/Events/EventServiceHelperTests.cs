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
    public class EventServiceHelperTests : IDisposable
    {
        // before
        public EventServiceHelperTests()
        {

        }

        // after
        public void Dispose()
        {

        }

        [Fact]
        public async Task PrepareEventWithRemoteAddressTest()
        {
            var options = new EventOptions
            {
                RaiseErrorEvents = true,
                RaiseFailureEvents = true,
                RaiseInformationEvents = true,
                RaiseSuccessEvents = true
            };

            var remoteIp = "127.0.0.1";
            var traceId = Guid.NewGuid().ToString();
            var procId = Process.GetCurrentProcess().Id;
            var subjectId = "123456789";
            var details = new EventDetails { Foo = "bar" };

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockConnectionInfo = new Mock<ConnectionInfo>();
            mockHttpContext.SetupGet(c => c.TraceIdentifier).Returns(traceId);
            mockConnectionInfo.SetupGet(c => c.RemoteIpAddress).Returns(System.Net.IPAddress.Parse(remoteIp));
            mockHttpContextAccessor.SetupGet(c => c.HttpContext).Returns(mockHttpContext.Object);
            mockHttpContext.SetupGet(c => c.Connection).Returns(mockConnectionInfo.Object);

            mockHttpContext.SetupGet(c => c.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", subjectId)
            })));

            var helper = new EventServiceHelper(options, mockHttpContextAccessor.Object);
            var eventIn = new Event<EventDetails>("category1", "success", EventTypes.Success, 4711, details, "Something nice is happened");
            var eventOut = helper.PrepareEvent(eventIn);

            eventOut.Should().Equals(eventIn);
            eventOut.Context.Should().NotBeNull();
            // TODO: eventOut.Context.MachineName
            eventOut.Context.ProcessId.Should().Be(procId);
            eventOut.Context.RemoteIpAddress.Should().Be(remoteIp);
            eventOut.Context.SubjectId.Should().Be(subjectId);

        }
    }

    public class EventDetails
    {
        public string Foo { get; set; }
    }
}
