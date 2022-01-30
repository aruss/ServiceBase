namespace ServiceBase.Notification.Smtp
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using ServiceBase;
    using ServiceBase.Notification.Email;

    /// <summary>
    ///
    /// </summary>
    public static partial class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="SmtpEmailSender"/> with <see cref="DefaultTokenizer"/>
        /// and <see cref="DefaultEmailService"/>. The <see cref="ITokenizer"/> and
        /// <see cref="IEmailService"/> can be replaced by adding those services prior to
        /// to calling this method. 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public static void AddDefaultEmailSmptSender(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger)
        {
            #region Add default email service 

            if (!services.IsAdded<ITokenizer>())
            {
                services.AddScoped<ITokenizer, DefaultTokenizer>();
            }

            if (!services.IsAdded<IEmailService>())
            {
                services.AddScoped<IEmailService, DefaultEmailService>();
                services.AddSingleton(new DefaultEmailServiceOptions());
            }

            #endregion

            #region Add SMTP email sender

            services.AddScoped<IEmailSender, SmtpEmailSender>();
            logger.LogDebug("Added SMTP mail sender");

            IConfigurationSection section =
                configuration.GetSection("Email:Smtp");

            if (section == null)
            {
                throw new NullReferenceException(
                    "Missing Email:Smtp configuration section");
            }

            services.AddSingleton(section.Get<SmtpOptions>());
            logger.LogDebug("Loaded SMTP configuration");

            #endregion 
        }
    }
}
