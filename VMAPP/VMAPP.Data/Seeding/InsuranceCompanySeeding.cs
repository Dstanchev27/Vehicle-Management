namespace VMAPP.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VMAPP.Data.Models;

    public class InsuranceCompanySeeding : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext)
        {
            if (dbContext.InsuranceCompanies.Any())
            {
                return;
            }

            var createdBy = dbContext.Users.FirstOrDefault();
            if (createdBy == null)
            {
                return;
            }

            var companies = new List<InsuranceCompany>
            {
                new InsuranceCompany
                {
                    Name = "SafeDrive Insurance",
                    Description = "Comprehensive auto insurance with competitive rates and fast claims",
                    City = "New York",
                    Address = "100 Liberty Street",
                    Email = "contact@safedrive.com",
                    Phone = "555-1001",
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new InsuranceCompany
                {
                    Name = "AutoShield Group",
                    Description = "Specialized vehicle protection plans for all vehicle types",
                    City = "Los Angeles",
                    Address = "200 Sunset Boulevard",
                    Email = "info@autoshield.com",
                    Phone = "555-1002",
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new InsuranceCompany
                {
                    Name = "Premier Motor Cover",
                    Description = "Premium insurance solutions for luxury and high-value vehicles",
                    City = "Chicago",
                    Address = "300 Michigan Avenue",
                    Email = "support@premiermotor.com",
                    Phone = "555-1003",
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new InsuranceCompany
                {
                    Name = "TrustAuto Insurance",
                    Description = "Reliable and affordable coverage for everyday drivers",
                    City = "Houston",
                    Address = "400 Energy Corridor",
                    Email = "hello@trustauto.com",
                    Phone = "555-1004",
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new InsuranceCompany
                {
                    Name = "GreenRoad Insurance",
                    Description = "Eco-friendly insurance plans with incentives for electric vehicles",
                    City = "San Francisco",
                    Address = "500 Market Street",
                    Email = "info@greenroad.com",
                    Phone = "555-1005",
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                }
            };

            await dbContext.InsuranceCompanies.AddRangeAsync(companies);
            await dbContext.SaveChangesAsync();
        }
    }
}
