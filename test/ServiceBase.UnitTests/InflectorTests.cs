namespace ServiceBase.UnitTests
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    public class InflectorTests
    {
        [Fact]
        public Task Dasherize()
        {
            "foo_bar_baz".Dasherize().Should().Be("foo-bar-baz");

            return Task.FromResult(0);
        }

        [Fact]
        public Task HumanizeUnderscored()
        {
            "foo_bar_baz".HumanizeUnderscored().Should().Be("foo bar baz");

            return Task.FromResult(0);
        }

        [Fact]
        public Task HumanizeDashed()
        {
            "foo-bar-baz".HumanizeDashed().Should().Be("foo bar baz");

            return Task.FromResult(0);
        }

        [Fact]
        public Task HumanizeCamelCased_WithUpperFirstChar()
        {
            "FooBarBaz".HumanizeCamelCased().Should().Be("foo bar baz");

            return Task.FromResult(0);
        }

        [Fact]
        public Task HumanizeCamelCased_WithLowerFirstChar()
        {
            "fooBarBaz".HumanizeCamelCased().Should().Be("foo bar baz");

            return Task.FromResult(0);
        }
    }
}
