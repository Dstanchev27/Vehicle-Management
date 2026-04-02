namespace VMAPP.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VMAPP.Data.Models;

    public class InsuranceClaimSeeding : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext)
        {
            if (dbContext.InsuranceClaims.Any())
            {
                return;
            }

            if (!dbContext.InsurancePolicies.Any())
            {
                return;
            }

            var claims = new List<InsuranceClaim>
            {
                new InsuranceClaim
                {
                    InsurancePolicyId = 1,
                    ClaimDate = DateTime.UtcNow.AddMonths(-10),
                    Description = "Front bumper damage from minor collision",
                    Amount = 1250.00m,
                    CreatedOn = DateTime.UtcNow
                },
                new InsuranceClaim
                {
                    InsurancePolicyId = 2,
                    ClaimDate = DateTime.UtcNow.AddMonths(-8),
                    Description = "Windshield replacement after road debris impact",
                    Amount = 450.00m,
                    CreatedOn = DateTime.UtcNow
                },
                new InsuranceClaim
                {
                    InsurancePolicyId = 3,
                    ClaimDate = DateTime.UtcNow.AddMonths(-6),
                    Description = "Side mirror and door panel repair after parking accident",
                    Amount = 870.50m,
                    CreatedOn = DateTime.UtcNow
                },
                new InsuranceClaim
                {
                    InsurancePolicyId = 4,
                    ClaimDate = DateTime.UtcNow.AddMonths(-5),
                    Description = "Hail damage repair across roof and hood",
                    Amount = 3200.00m,
                    CreatedOn = DateTime.UtcNow
                },
                new InsuranceClaim
                {
                    InsurancePolicyId = 5,
                    ClaimDate = DateTime.UtcNow.AddMonths(-4),
                    Description = "Rear-end collision repair including trunk and bumper",
                    Amount = 2100.75m,
                    CreatedOn = DateTime.UtcNow
                },
                new InsuranceClaim
                {
                    InsurancePolicyId = 6,
                    ClaimDate = DateTime.UtcNow.AddMonths(-3),
                    Description = "Theft of vehicle audio system and navigation unit",
                    Amount = 1500.00m,
                    CreatedOn = DateTime.UtcNow
                },
                new InsuranceClaim
                {
                    InsurancePolicyId = 7,
                    ClaimDate = DateTime.UtcNow.AddMonths(-2),
                    Description = "Flood damage to interior electronics and engine",
                    Amount = 5400.00m,
                    CreatedOn = DateTime.UtcNow
                },
                new InsuranceClaim
                {
                    InsurancePolicyId = 8,
                    ClaimDate = DateTime.UtcNow.AddMonths(-1),
                    Description = "Tyre blowout causing wheel arch damage",
                    Amount = 620.00m,
                    CreatedOn = DateTime.UtcNow
                },
                new InsuranceClaim
                {
                    InsurancePolicyId = 9,
                    ClaimDate = DateTime.UtcNow.AddDays(-20),
                    Description = "Hit-and-run damage to passenger side doors",
                    Amount = 1800.00m,
                    CreatedOn = DateTime.UtcNow
                },
                new InsuranceClaim
                {
                    InsurancePolicyId = 10,
                    ClaimDate = DateTime.UtcNow.AddDays(-10),
                    Description = "Fire damage to engine bay",
                    Amount = 7500.00m,
                    CreatedOn = DateTime.UtcNow
                }
            };

            await dbContext.InsuranceClaims.AddRangeAsync(claims);
            await dbContext.SaveChangesAsync();
        }
    }
}
