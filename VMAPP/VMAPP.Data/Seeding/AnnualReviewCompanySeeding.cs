namespace VMAPP.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VMAPP.Data.Models;

    public class AnnualReviewCompanySeeding : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext)
        {
            if (dbContext.AnnualReviewCompanies.Any())
            {
                return;
            }

            const string adminId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890";

            var companies = new List<AnnualReviewCompany>
            {
                new AnnualReviewCompany
                {
                    Name = "SafeCheck Annual Reviews",
                    Description = "Thorough annual vehicle inspections with certified technicians",
                    City = "New York",
                    Address = "10 Inspection Plaza",
                    Email = "contact@safecheck.com",
                    Phone = "555-2001",
                    CreatedById = adminId,
                    CreatedOn = DateTime.UtcNow
                },
                new AnnualReviewCompany
                {
                    Name = "AutoVerify Services",
                    Description = "Fast and reliable annual roadworthiness testing for all vehicle types",
                    City = "Los Angeles",
                    Address = "20 Verify Lane",
                    Email = "info@autoverify.com",
                    Phone = "555-2002",
                    CreatedById = adminId,
                    CreatedOn = DateTime.UtcNow
                },
                new AnnualReviewCompany
                {
                    Name = "National Vehicle Inspection Center",
                    Description = "Government-accredited annual vehicle inspection and certification",
                    City = "Chicago",
                    Address = "30 Compliance Avenue",
                    Email = "support@nvic.com",
                    Phone = "555-2003",
                    CreatedById = adminId,
                    CreatedOn = DateTime.UtcNow
                },
                new AnnualReviewCompany
                {
                    Name = "DriveReady Inspections",
                    Description = "Comprehensive safety and emissions checks for personal and commercial vehicles",
                    City = "Houston",
                    Address = "40 Roadworthy Street",
                    Email = "hello@driverready.com",
                    Phone = "555-2004",
                    CreatedById = adminId,
                    CreatedOn = DateTime.UtcNow
                },
                new AnnualReviewCompany
                {
                    Name = "GreenPass Vehicle Testing",
                    Description = "Eco-focused annual reviews with emphasis on emissions compliance",
                    City = "San Francisco",
                    Address = "50 Clean Air Blvd",
                    Email = "info@greenpass.com",
                    Phone = "555-2005",
                    CreatedById = adminId,
                    CreatedOn = DateTime.UtcNow
                }
            };

            await dbContext.AnnualReviewCompanies.AddRangeAsync(companies);
            await dbContext.SaveChangesAsync();
        }
    }
}
