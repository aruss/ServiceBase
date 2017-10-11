namespace Microsoft.Extensions.Logging
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Serilog;

    public static class IServiceCollectionLoggingExtensions
    {
        /// <summary>
        /// Adds Serilog logging services
        /// </summary>
        /// <param name="services">Instance of
        /// <see cref="IServiceCollection"/></param>
        /// <param name="config">Instance of <see cref="IConfiguration"/>
        /// </param>
        /// <returns>Instance of
        /// <see cref="IServiceCollection"/> provided via
        /// <paramref name="services"/> argument</returns>
        public static IServiceCollection AddSerilog(
            this IServiceCollection services,
            IConfiguration config)
        {
            return services.AddLogging(builder =>
            {
                builder.AddSerilog(config);
            });
        }

        /// <summary>
        /// Adds Serilog logger
        /// </summary>
        /// <param name="loggingBuilder">Instance of
        /// <see cref="ILoggingBuilder"/></param>
        /// <param name="config">Instance of <see cref="IConfiguration"/>
        /// </param>
        /// <returns>Instance of
        /// <see cref="ILoggingBuilder"/> provided via
        /// <paramref name="loggingBuilder"/> argument</returns>
        public static ILoggingBuilder AddSerilog(
            this ILoggingBuilder loggingBuilder,
            IConfiguration config)
        {
            var serilogOptions = config.GetSection("Serilog");

            var logger = new LoggerConfiguration()
                .ReadFrom.ConfigurationSection(serilogOptions)
                .CreateLogger();

            return loggingBuilder.AddSerilog(logger, true);
        }
    }
}