namespace VMAPP.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using VMAPP.Data.Models;

    public class ServiceRecordSeeding : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext)
        {
            if (dbContext.ServiceRecords.Any())
            {
                return;
            }

            var createdBy = dbContext.Users.FirstOrDefault();
            if (createdBy == null)
            {
                return;
            }

            var serviceRecords = new List<ServiceRecord>
            {
                new ServiceRecord
                {
                    ServiceDate = DateTime.UtcNow.AddMonths(-6),
                    Description = "Oil change and filter replacement",
                    Cost = 89.99m,
                    VehicleId = 1,
                    VehicleServiceId = 1,
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new ServiceRecord
                {
                    ServiceDate = DateTime.UtcNow.AddMonths(-5),
                    Description = "Brake pad replacement",
                    Cost = 299.99m,
                    VehicleId = 2,
                    VehicleServiceId = 3,
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new ServiceRecord
                {
                    ServiceDate = DateTime.UtcNow.AddMonths(-4),
                    Description = "Tire rotation and balancing",
                    Cost = 79.99m,
                    VehicleId = 3,
                    VehicleServiceId = 3,
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new ServiceRecord
                {
                    ServiceDate = DateTime.UtcNow.AddMonths(-3),
                    Description = "Battery replacement",
                    Cost = 189.99m,
                    VehicleId = 4,
                    VehicleServiceId = 4,
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new ServiceRecord
                {
                    ServiceDate = DateTime.UtcNow.AddMonths(-2),
                    Description = "Air conditioning service",
                    Cost = 129.99m,
                    VehicleId = 5,
                    VehicleServiceId = 2,
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new ServiceRecord
                {
                    ServiceDate = DateTime.UtcNow.AddMonths(-1),
                    Description = "Transmission fluid change",
                    Cost = 159.99m,
                    VehicleId = 6,
                    VehicleServiceId = 5,
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new ServiceRecord
                {
                    ServiceDate = DateTime.UtcNow.AddDays(-20),
                    Description = "Engine diagnostic and tune-up",
                    Cost = 249.99m,
                    VehicleId = 7,
                    VehicleServiceId = 6,
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new ServiceRecord
                {
                    ServiceDate = DateTime.UtcNow.AddDays(-15),
                    Description = "Wheel alignment",
                    Cost = 99.99m,
                    VehicleId = 8,
                    VehicleServiceId = 3,
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new ServiceRecord
                {
                    ServiceDate = DateTime.UtcNow.AddDays(-10),
                    Description = "Exhaust system repair",
                    Cost = 379.99m,
                    VehicleId = 9,
                    VehicleServiceId = 2,
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                },
                new ServiceRecord
                {
                    ServiceDate = DateTime.UtcNow.AddDays(-5),
                    Description = "Coolant flush and refill",
                    Cost = 119.99m,
                    VehicleId = 10,
                    VehicleServiceId = 1,
                    CreatedById = createdBy.Id,
                    CreatedOn = DateTime.UtcNow
                }
            };

            await dbContext.ServiceRecords.AddRangeAsync(serviceRecords);
            await dbContext.SaveChangesAsync();
        }
    }
}
