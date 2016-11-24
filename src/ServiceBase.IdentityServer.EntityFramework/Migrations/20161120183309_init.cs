using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ServiceBase.IdentityServer.EntityFramework.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(maxLength: 254, nullable: false),
                    EmailVerifiedAt = table.Column<DateTime>(nullable: true),
                    FailedLoginCount = table.Column<int>(nullable: false),
                    IsEmailVerified = table.Column<bool>(nullable: false),
                    IsLoginAllowed = table.Column<bool>(nullable: false),
                    LastFailedLoginAt = table.Column<DateTime>(nullable: true),
                    LastLoginAt = table.Column<DateTime>(nullable: true),
                    PasswordChangedAt = table.Column<DateTime>(nullable: true),
                    PasswordHash = table.Column<string>(maxLength: 200, nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    VerificationKey = table.Column<string>(maxLength: 100, nullable: true),
                    VerificationKeySentAt = table.Column<DateTime>(nullable: true),
                    VerificationPurpose = table.Column<int>(nullable: true),
                    VerificationStorage = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalAccounts",
                columns: table => new
                {
                    Provider = table.Column<string>(nullable: false),
                    Subject = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(maxLength: 250, nullable: true),
                    LastLoginAt = table.Column<DateTime>(nullable: true),
                    UserAccountId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalAccounts", x => new { x.Provider, x.Subject });
                    table.ForeignKey(
                        name: "FK_ExternalAccounts_UserAccounts_UserAccountId",
                        column: x => x.UserAccountId,
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    UserAccountId = table.Column<Guid>(nullable: false),
                    ValueType = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => new { x.UserId, x.Type, x.Value });
                    table.ForeignKey(
                        name: "FK_UserClaims_UserAccounts_UserAccountId",
                        column: x => x.UserAccountId,
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalAccounts_UserAccountId",
                table: "ExternalAccounts",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_Email",
                table: "UserAccounts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserAccountId",
                table: "UserClaims",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId_Type_Value",
                table: "UserClaims",
                columns: new[] { "UserId", "Type", "Value" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalAccounts");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserAccounts");
        }
    }
}
