using VMAPP.Data.Models.Enums;

namespace VMAPP.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using VMAPP.Data.Models;

    public class UserSeeding : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext)
        {
            const string insuranceUserId = "b2c3d4e5-f6a7-8901-bcde-f12345678901";
            const string serviceUserId = "c3d4e5f6-a7b8-9012-cdef-123456789012";

            if (dbContext.Users.Any(u => u.Id == insuranceUserId || u.Id == serviceUserId))
            {
                return;
            }

            var firstInsuranceCompany = dbContext.InsuranceCompanies.FirstOrDefault();
            var firstVehicleService = dbContext.VehicleServices.FirstOrDefault();

            var hasher = new PasswordHasher<ApplicationUser>();

            var insuranceUser = new ApplicationUser
            {
                Id = insuranceUserId,
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
                InsuranceCompanyId = firstInsuranceCompany?.Id,
            };

            var serviceUser = new ApplicationUser
            {
                Id = serviceUserId,
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
                VehicleServiceId = firstVehicleService?.Id,
            };

            insuranceUser.PasswordHash = hasher.HashPassword(insuranceUser, "Insurance123!");
            serviceUser.PasswordHash = hasher.HashPassword(serviceUser, "Service123!");

            await dbContext.Users.AddRangeAsync(insuranceUser, serviceUser);
            await dbContext.SaveChangesAsync();

            var roleMap = new[]
            {
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
