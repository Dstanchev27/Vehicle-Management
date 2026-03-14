namespace VMAPP.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using VMAPP.Data.Models;

    public class VehicleServiceSeeding : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext)
        {
            if (dbContext.VehicleServices.Any())
            {
                return;
            }

            var vehicleServices = new List<VehicleService>
            {
                new VehicleService
                {
                    Name = "Quick Lube Express",
                    Description = "Fast and efficient oil change services",
                    CreatedOn = DateTime.Now.AddYears(-3),
                    City = "New York",
                    Address = "123 Main Street",
                    Email = "info@quicklube.com",
                    Phone = "555-0101"
                },
                new VehicleService
                {
                    Name = "AutoCare Center",
                    Description = "Complete automotive repair and maintenance",
                    CreatedOn = DateTime.Now.AddYears(-5),
                    City = "Los Angeles",
                    Address = "456 Oak Avenue",
                    Email = "service@autocare.com",
                    Phone = "555-0102"
                },
                new VehicleService
                {
                    Name = "Premium Tire & Brake",
                    Description = "Specialized in tire and brake services",
                    CreatedOn = DateTime.Now.AddYears(-2),
                    City = "Chicago",
                    Address = "789 Elm Street",
                    Email = "contact@premiumtire.com",
                    Phone = "555-0103"
                },
                new VehicleService
                {
                    Name = "Electric Vehicle Solutions",
                    Description = "Expert service for electric and hybrid vehicles",
                    CreatedOn = DateTime.Now.AddYears(-1),
                    City = "San Francisco",
                    Address = "321 Tech Boulevard",
                    Email = "support@evsolutions.com",
                    Phone = "555-0104"
                },
                new VehicleService
                {
                    Name = "City Auto Repair",
                    Description = "Trusted neighborhood auto repair shop",
                    CreatedOn = DateTime.Now.AddYears(-4),
                    City = "Houston",
                    Address = "654 Park Lane",
                    Email = "hello@cityauto.com",
                    Phone = "555-0105"
                },
                new VehicleService
                {
                    Name = "Express Diagnostics",
                    Description = "Advanced computer diagnostics and repairs",
                    CreatedOn = DateTime.Now.AddYears(-2),
                    City = "Phoenix",
                    Address = "987 Innovation Drive",
                    Email = "info@expressdiag.com",
                    Phone = "555-0106"
                },
                new VehicleService
                {
                    Name = "Performance Plus",
                    Description = "High-performance tuning and modifications",
                    CreatedOn = DateTime.Now.AddYears(-3),
                    City = "Miami",
                    Address = "147 Speed Way",
                    Email = "contact@performanceplus.com",
                    Phone = "555-0107"
                },
                new VehicleService
                {
                    Name = "Family Auto Service",
                    Description = "Affordable service for all vehicle types",
                    CreatedOn = DateTime.Now.AddYears(-6),
                    City = "Seattle",
                    Address = "258 Community Road",
                    Email = "service@familyauto.com",
                    Phone = "555-0108"
                },
                new VehicleService
                {
                    Name = "Transmission Specialists",
                    Description = "Expert transmission repair and replacement",
                    CreatedOn = DateTime.Now.AddYears(-4),
                    City = "Boston",
                    Address = "369 Gear Street",
                    Email = "info@transspecialists.com",
                    Phone = "555-0109"
                },
                new VehicleService
                {
                    Name = "Green Auto Care",
                    Description = "Eco-friendly automotive services",
                    CreatedOn = DateTime.Now.AddYears(-1),
                    City = "Portland",
                    Address = "741 Eco Avenue",
                    Email = "hello@greenautocare.com",
                    Phone = "555-0110"
                }
            };

            await dbContext.VehicleServices.AddRangeAsync(vehicleServices);
            await dbContext.SaveChangesAsync();
        }
    }
}
