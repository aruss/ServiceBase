using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceBase.IdentityServer.EntityFramework.DbContexts;
using ServiceBase.IdentityServer.EntityFramework.Interfaces;
using ServiceBase.IdentityServer.EntityFramework.Options;
using ServiceBase.IdentityServer.EntityFramework.Services;
using ServiceBase.IdentityServer.EntityFramework.Stores;
using ServiceBase.IdentityServer.Services;
using System;
using System.Reflection;

namespace ServiceBase.IdentityServer.EntityFramework
{
    public static class IServiceCollectionExtensions
    {
        public static void AddEntityFrameworkStores(
          this IServiceCollection services,
          Action<EntityFrameworkOptions> configure = null)
        {
            services.Configure<EntityFrameworkOptions>(configure);

            var options = new EntityFrameworkOptions();
            if (configure != null)
            {
                configure(options);
            }
            ConfigureServices(services, options);
        }

        public static void AddEntityFrameworkStores(this IServiceCollection services, IConfigurationSection section)
        {
            services.Configure<EntityFrameworkOptions>(section);

            var options = new EntityFrameworkOptions();
            section.Bind(options);
            ConfigureServices(services, options);
        }

        internal static void SelectDatabase(this DbContextOptionsBuilder builder, EntityFrameworkOptions options)
        {
            if (String.IsNullOrWhiteSpace(options.ConnectionString))
            {
                builder.UseInMemoryDatabase();
            }
            else
            {
                var migrationsAssembly = typeof(IServiceCollectionExtensions).GetTypeInfo().Assembly.GetName().Name;
                builder.UseSqlServer(options.ConnectionString, o => o.MigrationsAssembly(migrationsAssembly));
            }
        }

        internal static void ConfigureServices(IServiceCollection services, EntityFrameworkOptions options)
        {
            Action<DbContextOptionsBuilder> builderOptions = (builder) =>
            {
                builder.SelectDatabase(options);
            };

            services.AddConfigurationStore(builderOptions);
            services.AddOperationalStore(builderOptions);
            services.AddUserAccountStore(builderOptions);

            // If db inialization or example data seeding is required add a default store initializer
            if (options.MigrateDatabase || options.SeedExampleData)
            {
                services.AddTransient<IStoreInitializer, DefaultStoreInitializer>();
            }

            var defaultStoreOptions = new UserAccountStoreOptions();
            services.AddSingleton(defaultStoreOptions);
        }

        internal static void AddConfigurationStore(
           this IServiceCollection services,
           Action<DbContextOptionsBuilder> dbContextOptionsAction = null,
           Action<ConfigurationStoreOptions> storeOptionsAction = null)
        {
            services.AddDbContext<ConfigurationDbContext>(dbContextOptionsAction);
            services.AddScoped<IConfigurationDbContext, ConfigurationDbContext>();

            services.AddTransient<IClientStore, ClientStore>();
            services.AddTransient<IResourceStore, ResourceStore>();
            services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            var options = new ConfigurationStoreOptions();
            storeOptionsAction?.Invoke(options);
            services.AddSingleton(options);
        }

        internal static void AddOperationalStore(
           this IServiceCollection services,
           Action<DbContextOptionsBuilder> dbContextOptionsAction = null,
           Action<PersistentGrantStoreOptions> storeOptionsAction = null,
           Action<TokenCleanupOptions> tokenCleanUpOptions = null)
        {
            services.AddDbContext<PersistedGrantDbContext>(dbContextOptionsAction);
            services.AddScoped<IPersistedGrantDbContext, PersistedGrantDbContext>();

            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            var storeOptions = new PersistentGrantStoreOptions();
            storeOptionsAction?.Invoke(storeOptions);
            services.AddSingleton(storeOptions);

            var tokenCleanupOptions = new TokenCleanupOptions();
            tokenCleanUpOptions?.Invoke(tokenCleanupOptions);
            services.AddSingleton(tokenCleanupOptions);
            services.AddSingleton<TokenCleanup>();
        }

        internal static void AddUserAccountStore(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> dbContextOptionsAction = null)
        {
            services.AddDbContext<UserAccountDbContext>(dbContextOptionsAction);
            services.AddScoped<UserAccountDbContext>();
            services.AddTransient<IUserAccountStore, UserAccountStore>();
        }
    }
}