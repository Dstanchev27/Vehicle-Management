using VMAPP.Data.Models.Enums;

namespace VMAPP.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using VMAPP.Data.Models;
    public class TestUsersSeeding : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext)
        {
            const string testAdminId = "d4e5f6a7-b8c9-0123-defa-234567890123";
            const string testInsuranceId = "e5f6a7b8-c9d0-1234-efab-345678901234";
            const string testServiceId = "f6a7b8c9-d0e1-2345-fabc-456789012345";
            const string testAnnualId = "a7b8c9d0-e1f2-3456-abcd-567890123456";

            if (dbContext.Users.Any(u =>
                    u.Id == testAdminId ||
                    u.Id == testInsuranceId ||
                    u.Id == testServiceId ||
                    u.Id == testAnnualId))
            {
                return;
            }

            var firstInsuranceCompany = dbContext.InsuranceCompanies.FirstOrDefault();
            var firstVehicleService = dbContext.VehicleServices.FirstOrDefault();
            var firstAnnualReviewCompany = dbContext.AnnualReviewCompanies.FirstOrDefault();

            var hasher = new PasswordHasher<ApplicationUser>();

            const string testAdminPassword = "TestAdmin123!";
            var testAdmin = new ApplicationUser
            {
                Id = testAdminId,
                UserName = "testadmin@vmapp.com",
                NormalizedUserName = "TESTADMIN@VMAPP.COM",
                Email = "testadmin@vmapp.com",
                NormalizedEmail = "TESTADMIN@VMAPP.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                City = "Sofia",
                Address = "1 Test Admin Street",
                CreatedOn = DateTime.UtcNow,
                TwoFactorEnabled = false,
            };
            testAdmin.PasswordHash = hasher.HashPassword(testAdmin, testAdminPassword);

            const string testInsurancePassword = "TestInsurance123!";
            var testInsurance = new ApplicationUser
            {
                Id = testInsuranceId,
                UserName = "testinsurance@vmapp.com",
                NormalizedUserName = "TESTINSURANCE@VMAPP.COM",
                Email = "testinsurance@vmapp.com",
                NormalizedEmail = "TESTINSURANCE@VMAPP.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                City = "Plovdiv",
                Address = "2 Test Insurance Avenue",
                CreatedOn = DateTime.UtcNow,
                TwoFactorEnabled = false,
                InsuranceCompanyId = firstInsuranceCompany?.Id,
            };
            testInsurance.PasswordHash = hasher.HashPassword(testInsurance, testInsurancePassword);

            const string testServicePassword = "TestService123!";
            var testService = new ApplicationUser
            {
                Id = testServiceId,
                UserName = "testservice@vmapp.com",
                NormalizedUserName = "TESTSERVICE@VMAPP.COM",
                Email = "testservice@vmapp.com",
                NormalizedEmail = "TESTSERVICE@VMAPP.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                City = "Varna",
                Address = "3 Test Mechanic Road",
                CreatedOn = DateTime.UtcNow,
                TwoFactorEnabled = false,
                VehicleServiceId = firstVehicleService?.Id,
            };
            testService.PasswordHash = hasher.HashPassword(testService, testServicePassword);

            const string testAnnualPassword = "TestAnnual123!";
            var testAnnual = new ApplicationUser
            {
                Id = testAnnualId,
                UserName = "testannual@vmapp.com",
                NormalizedUserName = "TESTANNUAL@VMAPP.COM",
                Email = "testannual@vmapp.com",
                NormalizedEmail = "TESTANNUAL@VMAPP.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                City = "Burgas",
                Address = "4 Test Review Boulevard",
                CreatedOn = DateTime.UtcNow,
                TwoFactorEnabled = false,
                AnnualReviewCompanyId = firstAnnualReviewCompany?.Id,
            };
            testAnnual.PasswordHash = hasher.HashPassword(testAnnual, testAnnualPassword);

            await dbContext.Users.AddRangeAsync(testAdmin, testInsurance, testService, testAnnual);
            await dbContext.SaveChangesAsync();

            var roleMap = new[]
            {
                (UserId: testAdmin.Id,     RoleName: nameof(AppRole.ProgramAdministrator)),
                (UserId: testInsurance.Id, RoleName: nameof(AppRole.InsuranceCompany)),
                (UserId: testService.Id,   RoleName: nameof(AppRole.VehicleService)),
                (UserId: testAnnual.Id,    RoleName: nameof(AppRole.AnnualReviewCompany)),
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
