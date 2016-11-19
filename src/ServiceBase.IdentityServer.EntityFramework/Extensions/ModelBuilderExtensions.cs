using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceBase.IdentityServer.EntityFramework.Entities;
using ServiceBase.IdentityServer.EntityFramework.Options;

namespace ServiceBase.IdentityServer.EntityFramework.Extensions
{
    public static class ModelBuilderExtensions
    {
        private static EntityTypeBuilder<TEntity> ToTable<TEntity>(
            this EntityTypeBuilder<TEntity> entityTypeBuilder,
            TableConfiguration configuration)
            where TEntity : class
        {
            return string.IsNullOrWhiteSpace(configuration.Schema) ?
                entityTypeBuilder.ToTable(configuration.Name) :
                entityTypeBuilder.ToTable(configuration.Name, configuration.Schema);
        }

        public static void ConfigureDefaultStore(this ModelBuilder modelBuilder, DefaultStoreOptions storeOptions)
        {
            if (!string.IsNullOrWhiteSpace(storeOptions.DefaultSchema)) modelBuilder.HasDefaultSchema(storeOptions.DefaultSchema);
            
            modelBuilder.Entity<UserAccount>(userAccount =>
            {
                userAccount.ToTable(storeOptions.UserAccount);
                userAccount.HasKey(x => new { x.Id });

                userAccount.Property(x => x.Email).HasMaxLength(254).IsRequired();
                userAccount.Property(x => x.IsEmailVerified).IsRequired();
                userAccount.Property(x => x.EmailVerifiedAt);

                userAccount.Property(x => x.IsLoginAllowed).IsRequired();
                userAccount.Property(x => x.LastLoginAt);
                userAccount.Property(x => x.FailedLoginCount);

                userAccount.Property(x => x.PasswordHash).HasMaxLength(200);
                userAccount.Property(x => x.PasswordChangedAt);

                userAccount.Property(x => x.VerificationKey).HasMaxLength(100);
                userAccount.Property(x => x.VerificationPurpose);
                userAccount.Property(x => x.VerificationKeySentAt);
                userAccount.Property(x => x.VerificationStorage).HasMaxLength(2000);

                userAccount.Property(x => x.CreatedAt);
                userAccount.Property(x => x.UpdatedAt);

                userAccount.HasMany(x => x.Accounts).WithOne(x => x.UserAccount)
                    .IsRequired().OnDelete(DeleteBehavior.Cascade);

                userAccount.HasMany(x => x.Claims).WithOne(x => x.UserAccount)
                    .IsRequired().OnDelete(DeleteBehavior.Cascade);

                userAccount.HasIndex(x => x.Email).IsUnique();
            });

            modelBuilder.Entity<ExternalAccount>(externalAccount =>
            {
                externalAccount.ToTable(storeOptions.ExternalAccount);
                externalAccount.HasKey(x => new { x.Provider, x.Subject });


                externalAccount.Property(x => x.Email).HasMaxLength(250);
                externalAccount.Property(x => x.LastLoginAt);
                externalAccount.Property(x => x.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<UserClaim>(userClaim =>
            {
                userClaim.ToTable(storeOptions.UserClaim);
                userClaim.HasKey(x => new { x.UserId, x.Type, x.Value });
                
                userClaim.Property(x => x.ValueType).HasMaxLength(2000);

                userClaim.HasIndex(x => new { x.UserId, x.Type, x.Value });
            });
        }
    }
}
