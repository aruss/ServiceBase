using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ServiceBase.IdentityServer.EntityFramework.DbContexts;

namespace ServiceBase.IdentityServer.EntityFramework.Migrations
{
    [DbContext(typeof(DefaultDbContext))]
    [Migration("20161118224117_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ServiceBase.IdentityServer.EntityFramework.Entities.ExternalAccount", b =>
                {
                    b.Property<string>("Provider");

                    b.Property<string>("Subject");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 250);

                    b.Property<DateTime?>("LastLoginAt");

                    b.Property<Guid?>("UserAccountId")
                        .IsRequired();

                    b.Property<Guid>("UserId");

                    b.HasKey("Provider", "Subject");

                    b.HasIndex("UserAccountId");

                    b.ToTable("ExternalAccounts");
                });

            modelBuilder.Entity("ServiceBase.IdentityServer.EntityFramework.Entities.UserAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 254);

                    b.Property<DateTime?>("EmailVerifiedAt");

                    b.Property<int>("FailedLoginCount");

                    b.Property<bool>("IsEmailVerified");

                    b.Property<bool>("IsLoginAllowed");

                    b.Property<DateTime?>("LastFailedLoginAt");

                    b.Property<DateTime?>("LastLoginAt");

                    b.Property<DateTime?>("PasswordChangedAt");

                    b.Property<string>("PasswordHash")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<string>("VerificationKey")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<DateTime?>("VerificationKeySentAt");

                    b.Property<int?>("VerificationPurpose");

                    b.Property<string>("VerificationStorage")
                        .HasAnnotation("MaxLength", 2000);

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("UserAccounts");
                });

            modelBuilder.Entity("ServiceBase.IdentityServer.EntityFramework.Entities.UserClaim", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<string>("Type");

                    b.Property<string>("Value");

                    b.Property<Guid?>("UserAccountId")
                        .IsRequired();

                    b.Property<string>("ValueType")
                        .HasAnnotation("MaxLength", 2000);

                    b.HasKey("UserId", "Type", "Value");

                    b.HasIndex("UserAccountId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("ServiceBase.IdentityServer.EntityFramework.Entities.ExternalAccount", b =>
                {
                    b.HasOne("ServiceBase.IdentityServer.EntityFramework.Entities.UserAccount", "UserAccount")
                        .WithMany("Accounts")
                        .HasForeignKey("UserAccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ServiceBase.IdentityServer.EntityFramework.Entities.UserClaim", b =>
                {
                    b.HasOne("ServiceBase.IdentityServer.EntityFramework.Entities.UserAccount", "UserAccount")
                        .WithMany("Claims")
                        .HasForeignKey("UserAccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
