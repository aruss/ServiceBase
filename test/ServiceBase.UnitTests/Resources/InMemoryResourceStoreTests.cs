namespace ServiceBase.UnitTests
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using ServiceBase.Resources;
    using Xunit;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using System;

    public class LoggerMock<TSource> : ILogger<TSource>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {

        }
    }

    [Collection("ServiceBase")]
    public class InMemoryResourceStoreTests
    {
        public InMemoryResourceStoreTests()
        {

        }

        [Fact]
        public async Task GetAllCulturesAsync()
        {
            IResourceStore store = new InMemoryResourceStore(
                new LoggerMock<InMemoryResourceStore>());

            await store.WriteAsync("de-DE", "Group1", "Key1", "GermanValue1");
            await store.WriteAsync("de-DE", "Group1", "Key2", "GermanValue2");
            await store.WriteAsync("en-US", "Group1", "Key1", "EnglishValue1");
            await store.WriteAsync("en-US", "Group1", "Key2", "EnglishValue2");

            await store.WriteAsync("de-DE", "Group2", "Key3", "GermanValue3");
            await store.WriteAsync("de-DE", "Group2", "Key4", "GermanValue4");
            await store.WriteAsync("en-US", "Group2", "Key3", "EnglishValue3");
            await store.WriteAsync("en-US", "Group2", "Key4", "EnglishValue4");

            string[] cultures = (await store.GetAllCulturesAsync("Group1")).ToArray();

            cultures.Length.Should().Be(2);
            cultures.FirstOrDefault(c => c.Equals("de-DE")).Should().NotBeNull();
            cultures.FirstOrDefault(c => c.Equals("en-US")).Should().NotBeNull();
        }

        private void AssertResource(
            Resource[] resources,
            string key,
            string cultureExpected,
            string groupExpected,
            string keyExpected,
            string valueExpected,
            string sourceExpected = null)
        {
            Resource resource = resources.FirstOrDefault(s => s.Key.Equals(key));

            resource.Should().NotBeNull();

            resource.Culture.Should().Be(cultureExpected);
            resource.Group.Should().Be(groupExpected);
            resource.Key.Should().Be(keyExpected);
            resource.Value.Should().Be(valueExpected);
            resource.Source.Should().Be(sourceExpected);
        }
    }
}
