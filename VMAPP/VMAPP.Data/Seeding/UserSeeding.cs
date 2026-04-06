namespace VMAPP.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;

    using VMAPP.Data.Models;
    using VMAPP.Data.Models.Enums;

    public class UserSeeding : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext)
        {
            if (dbContext.Users.Any())
            {
                return;
            }

            var hasher = new PasswordHasher<ApplicationUser>();

            var admin = new ApplicationUser
            {
                Id = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
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

            var insuranceUser = new ApplicationUser
            {
                Id = "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                UserName = "insurance@vmapp.com",
                NormalizedUserName = "INSURANCE@VMAPP.COM",
                Email = "insurance@vmapp.com",
                NormalizedEmail = "INSURANCE@VMAPP.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                City = "Plovdiv",
                Address = "15 Insurance Avenue",
                CreatedOn = DateTime.UtcNow,
                TwoFactorEnabled = false,
            };

            var serviceUser = new ApplicationUser
            {
                Id = "c3d4e5f6-a7b8-9012-cdef-123456789012",
                UserName = "service@vmapp.com",
                NormalizedUserName = "SERVICE@VMAPP.COM",
                Email = "service@vmapp.com",
                NormalizedEmail = "SERVICE@VMAPP.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                City = "Varna",
                Address = "7 Mechanic Road",
                CreatedOn = DateTime.UtcNow,
                TwoFactorEnabled = false,
            };

            admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");
            insuranceUser.PasswordHash = hasher.HashPassword(insuranceUser, "Insurance123!");
            serviceUser.PasswordHash = hasher.HashPassword(serviceUser, "Service123!");

            await dbContext.Users.AddRangeAsync(admin, insuranceUser, serviceUser);
            await dbContext.SaveChangesAsync();

            var roleMap = new[]
            {
                (UserId: admin.Id, RoleName: nameof(AppRole.ProgramAdministrator)),
                (UserId: insuranceUser.Id, RoleName: nameof(AppRole.InsuranceCompany)),
                (UserId: serviceUser.Id, RoleName: nameof(AppRole.VehicleService)),
            };

            foreach (var (userId, roleName) in roleMap)
            {
                var role = dbContext.Roles.FirstOrDefault(r => r.Name == roleName);
                if (role != null)
                {
                    await dbContext.UserRoles.AddAsync(new IdentityUserRole<string>
                    {
                        UserId = userId,
                        RoleId = role.Id,
                    });
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
