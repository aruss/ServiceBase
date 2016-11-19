using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Services;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceBase.IdentityServer.EntityFramework.DbContexts;
using ServiceBase.IdentityServer.EntityFramework.Extensions;
using ServiceBase.IdentityServer.EntityFramework.Options;
using ServiceBase.IdentityServer.Services;
using System;
using System.Reflection;

namespace ServiceBase.IdentityServer.EntityFramework
{
    public static class IServiceCollectionExtensions
    {
        public static void AddEntityFrameworkStores(
            this IServiceCollection services,
            Action<EntityFrameworkOptions> configure)
        {
            services.Configure<EntityFrameworkOptions>(configure);

            var options = new EntityFrameworkOptions();
            configure(options);
            ConfigureServices(services, options);
        }

        public static void AddEntityFrameworkStores(this IServiceCollection services, IConfigurationSection section)
        {
            services.Configure<EntityFrameworkOptions>(section);

            var options = new EntityFrameworkOptions();
            section.Bind(options);
            ConfigureServices(services, options);
        }

        internal static void ConfigureServices(IServiceCollection services, EntityFrameworkOptions options)
        {
            var migrationsAssembly = typeof(IServiceCollectionExtensions).GetTypeInfo().Assembly.GetName().Name;

            // AddConfigurationStore
            services.AddDbContext<ConfigurationDbContext>(builder =>
                builder.UseSqlServer(options.ConnectionString, o => o.MigrationsAssembly(migrationsAssembly)));

            services.AddScoped<IConfigurationDbContext, ConfigurationDbContext>();
            services.AddTransient<IClientStore, ClientStore>();
            services.AddTransient<IScopeStore, ScopeStore>();
            services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            var configStoreOptions = new ConfigurationStoreOptions();
            //configStoreOptions.SetPrefix(options.TablePrefix); 
            services.AddSingleton(configStoreOptions);

            // AddOperationalStore
            services.AddDbContext<PersistedGrantDbContext>(builder =>
                builder.UseSqlServer(options.ConnectionString, o => o.MigrationsAssembly(migrationsAssembly)));

            services.AddScoped<IPersistedGrantDbContext, PersistedGrantDbContext>();
            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            var operationStoreOptions = new OperationalStoreOptions();
            //operationStoreOptions.SetPrefix(options.TablePrefix); 
            services.AddSingleton(operationStoreOptions);

            // TODO: take care of token cleanup
            /*var tokenCleanupOptions = new TokenCleanupOptions();
            tokenCleanUpOptions?.Invoke(tokenCleanupOptions);
            builder.Services.AddSingleton(tokenCleanupOptions);
            builder.Services.AddSingleton<TokenCleanup>();*/

            services.AddDbContext<DefaultDbContext>(builder =>
                builder.UseSqlServer(options.ConnectionString, o => o.MigrationsAssembly(migrationsAssembly)));

            services.AddScoped<DefaultDbContext>();
            services.AddTransient<IUserAccountStore, UserAccountStore>();
            services.AddTransient<IStoreInitializer, StoreInitializer>();

            var defaultStoreOptions = new DefaultStoreOptions();            
            services.AddSingleton(defaultStoreOptions);
        }
    }
}
