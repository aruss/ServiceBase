namespace ServiceBase.UnitTests
{
    using FluentAssertions;
    using System.Threading.Tasks;
    using Xunit;

    [Collection("ServiceBase")]
    public class InflectorTests
    {
        [Fact]
        public Task DasherizeTest()
        {
            "foo_bar_baz".Dasherize().Should().Be("foo-bar-baz");

            return Task.FromResult(0); 
        }
    }
}
