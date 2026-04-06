using VMAPP.Data.Models.Enums;

namespace VMAPP.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;

    using VMAPP.Data.Models;

    public class AdminUserSeeding : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext)
        {
            const string adminId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890";

            if (dbContext.Users.Any(u => u.Id == adminId))
            {
                return;
            }

            var hasher = new PasswordHasher<ApplicationUser>();

            var admin = new ApplicationUser
            {
                Id = adminId,
                UserName = "admin@vmapp.com",
                NormalizedUserName = "ADMIN@VMAPP.COM",
                Email = "admin@vmapp.com",
                NormalizedEmail = "ADMIN@VMAPP.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                City = "Sofia",
                Address = "1 Admin Street",
                CreatedOn = DateTime.UtcNow,
                TwoFactorEnabled = false,
            };

            admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

            await dbContext.Users.AddAsync(admin);
            await dbContext.SaveChangesAsync();

            var role = dbContext.Roles.FirstOrDefault(r => r.Name == nameof(AppRole.ProgramAdministrator));
            if (role != null)
            {
                await dbContext.UserRoles.AddAsync(new IdentityUserRole<string>
                {
                    UserId = admin.Id,
                    RoleId = role.Id,
                });

                await dbContext.SaveChangesAsync();
            }
        }
    }
}