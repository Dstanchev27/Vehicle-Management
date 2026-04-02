namespace VMAPP.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VMAPP.Data.Models;

    public class InsurancePolicySeeding : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext)
        {
            if (dbContext.InsurancePolicies.Any())
            {
                return;
            }

            if (!dbContext.Vehicles.Any() || !dbContext.InsuranceCompanies.Any())
            {
                return;
            }

            var createdBy = dbContext.Users.FirstOrDefault();
            if (createdBy == null)
            {
                return;
            }

            var policies = new List<InsurancePolicy>
            {
                new InsurancePolicy
                {
                    VehicleId = 1,
                    InsuranceCompanyId = 1,
                    PolicyNumber = "POL-2024-00001",
                    StartDate = DateTime.UtcNow.AddMonths(-12),
                    EndDate = DateTime.UtcNow.AddMonths(12),
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new InsurancePolicy
                {
                    VehicleId = 2,
                    InsuranceCompanyId = 1,
                    PolicyNumber = "POL-2024-00002",
                    StartDate = DateTime.UtcNow.AddMonths(-10),
                    EndDate = DateTime.UtcNow.AddMonths(14),
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new InsurancePolicy
                {
                    VehicleId = 3,
                    InsuranceCompanyId = 2,
                    PolicyNumber = "POL-2024-00003",
                    StartDate = DateTime.UtcNow.AddMonths(-8),
                    EndDate = DateTime.UtcNow.AddMonths(4),
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new InsurancePolicy
                {
                    VehicleId = 4,
                    InsuranceCompanyId = 2,
                    PolicyNumber = "POL-2024-00004",
                    StartDate = DateTime.UtcNow.AddMonths(-6),
                    EndDate = DateTime.UtcNow.AddMonths(6),
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new InsurancePolicy
                {
                    VehicleId = 5,
                    InsuranceCompanyId = 3,
                    PolicyNumber = "POL-2024-00005",
                    StartDate = DateTime.UtcNow.AddMonths(-12),
                    EndDate = DateTime.UtcNow.AddMonths(12),
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new InsurancePolicy
                {
                    VehicleId = 6,
                    InsuranceCompanyId = 3,
                    PolicyNumber = "POL-2024-00006",
                    StartDate = DateTime.UtcNow.AddMonths(-3),
                    EndDate = DateTime.UtcNow.AddMonths(21),
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new InsurancePolicy
                {
                    VehicleId = 7,
                    InsuranceCompanyId = 4,
                    PolicyNumber = "POL-2024-00007",
                    StartDate = DateTime.UtcNow.AddMonths(-9),
                    EndDate = DateTime.UtcNow.AddMonths(3),
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new InsurancePolicy
                {
                    VehicleId = 8,
                    InsuranceCompanyId = 4,
                    PolicyNumber = "POL-2024-00008",
                    StartDate = DateTime.UtcNow.AddMonths(-6),
                    EndDate = DateTime.UtcNow.AddMonths(6),
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new InsurancePolicy
                {
                    VehicleId = 9,
                    InsuranceCompanyId = 5,
                    PolicyNumber = "POL-2024-00009",
                    StartDate = DateTime.UtcNow.AddMonths(-12),
                    EndDate = DateTime.UtcNow.AddMonths(12),
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new InsurancePolicy
                {
                    VehicleId = 10,
                    InsuranceCompanyId = 5,
                    PolicyNumber = "POL-2024-00010",
                    StartDate = DateTime.UtcNow.AddMonths(-4),
                    EndDate = DateTime.UtcNow.AddMonths(20),
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                }
            };

            await dbContext.InsurancePolicies.AddRangeAsync(policies);
            await dbContext.SaveChangesAsync();
        }
    }
}
