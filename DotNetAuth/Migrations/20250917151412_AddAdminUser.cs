using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetAuth.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create password hasher
            var hasher = new PasswordHasher<IdentityUser>();

            // Create admin user
            var adminId = Guid.NewGuid().ToString();
            var securityStamp = Guid.NewGuid().ToString("D");
            
            // Hash the password
            var passwordHash = hasher.HashPassword(null, "Greenbacca2003!");

            // Insert into AspNetUsers table with all required columns
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { 
                    "Id", 
                    "UserName", 
                    "NormalizedUserName", 
                    "Email", 
                    "NormalizedEmail", 
                    "EmailConfirmed", 
                    "PasswordHash", 
                    "SecurityStamp",
                    "PhoneNumber",
                    "PhoneNumberConfirmed",
                    "TwoFactorEnabled",
                    "LockoutEnabled",
                    "AccessFailedCount",
                    "ConcurrencyStamp"
                },
                values: new object[] { 
                    adminId, 
                    "admin", 
                    "ADMIN", 
                    "admin@example.com", 
                    "ADMIN@EXAMPLE.COM", 
                    true, 
                    passwordHash, 
                    securityStamp,
                    null,           // PhoneNumber
                    false,          // PhoneNumberConfirmed
                    false,          // TwoFactorEnabled
                    false,          // LockoutEnabled
                    0,              // AccessFailedCount
                    Guid.NewGuid().ToString("D") // ConcurrencyStamp
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the admin user
            migrationBuilder.Sql("DELETE FROM \"AspNetUsers\" WHERE \"UserName\" = 'admin'");
        }
    }
}