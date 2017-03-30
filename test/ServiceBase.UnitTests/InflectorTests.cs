using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace ServiceBase.UnitTests
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
