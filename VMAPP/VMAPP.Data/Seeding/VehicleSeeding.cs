using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMAPP.Data.Models;
using VMAPP.Data.Models.Enums;

namespace VMAPP.Data.Seeding
{
    public class VehicleSeeding : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext)
        {
            if (dbContext.Vehicles.Any())
            {
                return;
            }

            var vehicles = new List<Vehicle>
            {
                new Vehicle
                {
                    VIN = "1HGBH41JXMN109186",
                    CarBrand = "Toyota",
                    CarModel = "Camry",
                    CreatedOnYear = 2020,
                    Color = "Silver",
                    VehicleType = VehicleType.Sedan,
                    CreatedOn = DateTime.UtcNow
                },
                new Vehicle
                {
                    VIN = "2C3CDXHG8LH123456",
                    CarBrand = "Honda",
                    CarModel = "CR-V",
                    CreatedOnYear = 2021,
                    Color = "Blue",
                    VehicleType = VehicleType.SUV,
                    CreatedOn = DateTime.UtcNow
                },
                new Vehicle
                {
                    VIN = "3FADP4EJ9FM234567",
                    CarBrand = "Ford",
                    CarModel = "F-150",
                    CreatedOnYear = 2019,
                    Color = "Black",
                    VehicleType = VehicleType.Truck,
                    CreatedOn = DateTime.UtcNow
                },
                new Vehicle
                {
                    VIN = "5YJSA1E26HF345678",
                    CarBrand = "Tesla",
                    CarModel = "Model S",
                    CreatedOnYear = 2022,
                    Color = "White",
                    VehicleType = VehicleType.Electric,
                    CreatedOn = DateTime.UtcNow
                },
                new Vehicle
                {
                    VIN = "WBADT43452G456789",
                    CarBrand = "BMW",
                    CarModel = "3 Series",
                    CreatedOnYear = 2020,
                    Color = "Gray",
                    VehicleType = VehicleType.Sedan,
                    CreatedOn = DateTime.UtcNow
                },
                new Vehicle
                {
                    VIN = "1C4RJFBG1FC567890",
                    CarBrand = "Jeep",
                    CarModel = "Grand Cherokee",
                    CreatedOnYear = 2021,
                    Color = "Red",
                    VehicleType = VehicleType.SUV,
                    CreatedOn = DateTime.UtcNow
                },
                new Vehicle
                {
                    VIN = "2HGFC2F53LH678901",
                    CarBrand = "Honda",
                    CarModel = "Civic",
                    CreatedOnYear = 2023,
                    Color = "Green",
                    VehicleType = VehicleType.Sedan,
                    CreatedOn = DateTime.UtcNow
                },
                new Vehicle
                {
                    VIN = "5FNRL6H78MB789012",
                    CarBrand = "Honda",
                    CarModel = "Odyssey",
                    CreatedOnYear = 2022,
                    Color = "Silver",
                    VehicleType = VehicleType.Van,
                    CreatedOn = DateTime.UtcNow
                },
                new Vehicle
                {
                    VIN = "KNDJP3A54K7890123",
                    CarBrand = "Kia",
                    CarModel = "Soul",
                    CreatedOnYear = 2019,
                    Color = "Orange",
                    VehicleType = VehicleType.Hatchback,
                    CreatedOn = DateTime.UtcNow
                },
                new Vehicle
                {
                    VIN = "JM1BL1SF7A1901234",
                    CarBrand = "Mazda",
                    CarModel = "Mazda3",
                    CreatedOnYear = 2020,
                    Color = "Blue",
                    VehicleType = VehicleType.Sedan,
                    CreatedOn = DateTime.UtcNow
                }
            };

            await dbContext.Vehicles.AddRangeAsync(vehicles);
            await dbContext.SaveChangesAsync();
        }
    }
}
