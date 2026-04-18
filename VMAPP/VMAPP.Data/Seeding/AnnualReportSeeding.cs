namespace VMAPP.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VMAPP.Data.Models;

    public class AnnualReportSeeding : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext)
        {
            if (dbContext.AnnualReports.Any())
            {
                return;
            }

            if (!dbContext.Vehicles.Any() || !dbContext.AnnualReviewCompanies.Any())
            {
                return;
            }

            var createdBy = dbContext.Users.FirstOrDefault();
            if (createdBy == null)
            {
                return;
            }

            var reports = new List<AnnualReport>
            {
                new AnnualReport
                {
                    VehicleId = 1,
                    AnnualReviewCompanyId = 1,
                    ReportNumber = "AR-2024-00001",
                    InspectionDate = DateTime.UtcNow.AddMonths(-11),
                    ExpiryDate = DateTime.UtcNow.AddMonths(1),
                    Passed = true,
                    Notes = "All systems operational. Minor tyre wear noted.",
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new AnnualReport
                {
                    VehicleId = 2,
                    AnnualReviewCompanyId = 1,
                    ReportNumber = "AR-2024-00002",
                    InspectionDate = DateTime.UtcNow.AddMonths(-9),
                    ExpiryDate = DateTime.UtcNow.AddMonths(3),
                    Passed = true,
                    Notes = "Vehicle passed all safety and emissions checks.",
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new AnnualReport
                {
                    VehicleId = 3,
                    AnnualReviewCompanyId = 2,
                    ReportNumber = "AR-2024-00003",
                    InspectionDate = DateTime.UtcNow.AddMonths(-6),
                    ExpiryDate = DateTime.UtcNow.AddMonths(6),
                    Passed = false,
                    Notes = "Failed emissions test. Repair required before re-inspection.",
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new AnnualReport
                {
                    VehicleId = 4,
                    AnnualReviewCompanyId = 3,
                    ReportNumber = "AR-2024-00004",
                    InspectionDate = DateTime.UtcNow.AddMonths(-3),
                    ExpiryDate = DateTime.UtcNow.AddMonths(9),
                    Passed = true,
                    Notes = "Full pass. Brake pads replaced prior to inspection.",
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new AnnualReport
                {
                    VehicleId = 5,
                    AnnualReviewCompanyId = 4,
                    ReportNumber = "AR-2024-00005",
                    InspectionDate = DateTime.UtcNow.AddMonths(-1),
                    ExpiryDate = DateTime.UtcNow.AddMonths(11),
                    Passed = true,
                    Notes = "No issues found. Vehicle in excellent condition.",
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                }
            };

            await dbContext.AnnualReports.AddRangeAsync(reports);
            await dbContext.SaveChangesAsync();
        }
    }
}
