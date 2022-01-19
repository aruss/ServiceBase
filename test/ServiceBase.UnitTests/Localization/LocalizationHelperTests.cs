
namespace ServiceBase.UnitTests.Localization
{
    using FluentAssertions;
    using ServiceBase.Localization;
    using Xunit;

    public class LocalizationHelperTests
    {
        [Theory]
        [InlineData("de-de", "/en-us", "/de-de")]
        [InlineData("de-de", "/en-us?foo=bar", "/de-de?foo=bar")]
        [InlineData("de-de", "/en-us/?foo=bar", "/de-de/?foo=bar")]
        [InlineData("de-de", "/en-us/privacy?foo=bar", "/de-de/privacy?foo=bar")]
        [InlineData("de-de", "/en-us/privacy/?foo=bar", "/de-de/privacy/?foo=bar")]
        public void ReplaceCulture(string culture, string currentUrl, string expected)
        {
            string actual = LocalizationHelper.ReplaceCulture(currentUrl, culture);
            actual.Should().Be(expected);
        }
    }
}
