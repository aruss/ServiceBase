namespace ServiceBase.UnitTests
{
    using System;
    using System.Diagnostics;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using ServiceBase;
    using ServiceBase.Events;
    using Xunit;

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
        public async Task PrepareEventWithRemoteAddress()
        {
            var options = new EventOptions
            {
                RaiseErrorEvents = true,
                RaiseFailureEvents = true,
                RaiseInformationEvents = true,
                RaiseSuccessEvents = true
            };

            string remoteIp = "127.0.0.1";
            string traceId = Guid.NewGuid().ToString();
            int procId = Process.GetCurrentProcess().Id;
            string subjectId = "123456789";
            FooEvent fooEvent = new FooEvent(
                "FooEvents",
                "FooEvent",
                EventTypes.Information,
                1337,
                "Some foo message")
            {
                Foo = "bar"
            };

            Mock<IHttpContextAccessor> mockHttpContextAccessor =
                new Mock<IHttpContextAccessor>();

            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();

            Mock<ConnectionInfo> mockConnectionInfo =
                new Mock<ConnectionInfo>();

            mockHttpContext.SetupGet(c => c.TraceIdentifier).Returns(traceId);

            mockConnectionInfo.SetupGet(c => c.RemoteIpAddress)
                .Returns(System.Net.IPAddress.Parse(remoteIp));

            mockHttpContextAccessor.SetupGet(c => c.HttpContext)
                .Returns(mockHttpContext.Object);

            mockHttpContext.SetupGet(c => c.Connection)
                .Returns(mockConnectionInfo.Object);
            
            mockHttpContext.SetupGet(c => c.User)
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", subjectId)
            })));

            Mock<IEventSink> mockEventSink = new Mock<IEventSink>();

            DefaultEventService eventService = new DefaultEventService(
                options,
                mockHttpContextAccessor.Object,
                new DateTimeAccessor(),
                mockEventSink.Object);

            await eventService.RaiseAsync(fooEvent);


            /*eventOut.Should().Equals(eventIn);
            eventOut.Context.Should().NotBeNull();
            eventOut.Context.ProcessId.Should().Be(procId);
            eventOut.Context.RemoteIpAddress.Should().Be(remoteIp);
            eventOut.Context.SubjectId.Should().Be(subjectId);*/
        }
    }

    public class FooEvent : Event
    {
        public FooEvent(
            string category,
            string name,
            EventTypes type,
            int id,
            string message = null)
            : base(category, name, type, id, message)
        {
        }

        public string Foo { get; set; }
    }
}