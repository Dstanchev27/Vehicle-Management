using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VMAPP.Data;
using VMAPP.Data.Models;
using VMAPP.Data.Models.Enums;
using VMAPP.Services;
using VMAPP.Services.DTOs.VehicleDTOs;
using VMAPP.Services.DTOs.VehicleServiceDTOs;

namespace VMAPP.Test.Services
{
    [TestFixture]
    public class VSCarsServiceTests
    {
        private ApplicationDbContext dbContext = null!;
        private VSCarsService service = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            dbContext = new ApplicationDbContext(options);
            service = new VSCarsService(dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        [Test]
        public async Task GetServiceWithVehiclesByNameAsync_WhenNameIsNullOrWhiteSpace_ReturnsNull()
        {
            var result = await service.GetServiceWithVehiclesByNameAsync("   ");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetServiceWithVehiclesByNameAsync_WhenServiceNotFound_ReturnsNull()
        {
            var result = await service.GetServiceWithVehiclesByNameAsync("Unknown");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetServiceWithVehiclesByNameAsync_WhenServiceExists_ReturnsDtoWithName()
        {
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            var result = await service.GetServiceWithVehiclesByNameAsync("AutoFix");

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("AutoFix"));
        }

        [Test]
        public async Task AddVehicleToServiceAsync_AddsVehicleAndReturnsNewId()
        {
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            var dto = new VehicleDto { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan };

            var id = await service.AddVehicleToServiceAsync(vs.Id, dto);

            Assert.That(id, Is.GreaterThan(0));
            Assert.That(await dbContext.Vehicles.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetVehicleDetailsAsync_WhenVehicleNotFound_ReturnsNull()
        {
            var result = await service.GetVehicleDetailsAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetVehicleDetailsAsync_WhenVehicleExists_ReturnsDetailsWithServiceRecords()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            dbContext.ServiceRecords.Add(new ServiceRecord { VehicleId = vehicle.VehicleId, VehicleServiceId = vs.Id, Cost = 100, Description = "Oil change", ServiceDate = DateTime.UtcNow, CreatedById = "user1", CreatedOn = DateTime.UtcNow });
            await dbContext.SaveChangesAsync();

            var result = await service.GetVehicleDetailsAsync(vehicle.VehicleId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Vehicle.CarBrand, Is.EqualTo("Toyota"));
            Assert.That(result.ServiceRecords, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task UpdateVehicleAsync_WhenVehicleExists_UpdatesAndReturnsTrue()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            await dbContext.SaveChangesAsync();

            var dto = new VehicleDto { Id = vehicle.VehicleId, VIN = "22222222222222222", CarBrand = "BMW", CarModel = "X5", CreatedOnYear = 2021, Color = "Blue", VehicleType = VehicleType.SUV };

            var result = await service.UpdateVehicleAsync(dto);

            Assert.That(result, Is.True);
            var updated = await dbContext.Vehicles.FindAsync(vehicle.VehicleId);
            Assert.That(updated!.CarBrand, Is.EqualTo("BMW"));
        }

        [Test]
        public async Task UpdateVehicleAsync_WhenVehicleNotFound_ReturnsFalse()
        {
            var dto = new VehicleDto { Id = 999, VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red" };

            var result = await service.UpdateVehicleAsync(dto);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteVehicleAsync_WhenVehicleExists_DeletesAndReturnsTrue()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            await dbContext.SaveChangesAsync();

            var result = await service.DeleteVehicleAsync(vehicle.VehicleId);

            Assert.That(result, Is.True);
            Assert.That(await dbContext.Vehicles.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteVehicleAsync_WhenVehicleNotFound_ReturnsFalse()
        {
            var result = await service.DeleteVehicleAsync(999);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetVehicleWithServiceRecordsAsync_ReturnsVehicleWithFilteredRecords()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var vs1 = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            var vs2 = new VehicleService { Name = "SpeedShop", Description = "Tuning", City = "Plovdiv", Address = "Str 2", Email = "b@b.com", Phone = "0888111111", CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            dbContext.VehicleServices.AddRange(vs1, vs2);
            await dbContext.SaveChangesAsync();

            dbContext.ServiceRecords.AddRange(
                new ServiceRecord { VehicleId = vehicle.VehicleId, VehicleServiceId = vs1.Id, Cost = 100, Description = "Oil change", ServiceDate = DateTime.UtcNow, CreatedById = "user1", CreatedOn = DateTime.UtcNow },
                new ServiceRecord { VehicleId = vehicle.VehicleId, VehicleServiceId = vs2.Id, Cost = 200, Description = "Brakes", ServiceDate = DateTime.UtcNow, CreatedById = "user1", CreatedOn = DateTime.UtcNow }
            );
            await dbContext.SaveChangesAsync();

            var result = await service.GetVehicleWithServiceRecordsAsync(vehicle.VehicleId, vs1.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.VIN, Is.EqualTo("11111111111111111"));
            Assert.That(result.ServiceRecords, Has.Count.EqualTo(1));
            Assert.That(result.ServiceRecords[0].Cost, Is.EqualTo(100));
        }

        [Test]
        public async Task GetServiceRecordByIdAsync_WhenExists_ReturnsDto()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            var sr = new ServiceRecord { VehicleId = vehicle.VehicleId, VehicleServiceId = vs.Id, Cost = 150, Description = "Tires", ServiceDate = DateTime.UtcNow, CreatedById = "user1", CreatedOn = DateTime.UtcNow };
            dbContext.ServiceRecords.Add(sr);
            await dbContext.SaveChangesAsync();

            var result = await service.GetServiceRecordByIdAsync(sr.ServiceRecordId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Cost, Is.EqualTo(150));
        }

        [Test]
        public async Task GetServiceRecordByIdAsync_WhenNotFound_ReturnsNull()
        {
            var result = await service.GetServiceRecordByIdAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task AddServiceRecordAsync_AddsRecordAndReturnsId()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            var dto = new ServiceRecordDto { VehicleId = vehicle.VehicleId, VehicleServiceId = vs.Id, Cost = 200, Description = "Alignment", ServiceDate = DateTime.UtcNow, CreatedById = "user1" };

            var id = await service.AddServiceRecordAsync(dto);

            Assert.That(id, Is.GreaterThan(0));
            Assert.That(await dbContext.ServiceRecords.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateServiceRecordAsync_WhenExists_UpdatesAndReturnsTrue()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            var sr = new ServiceRecord { VehicleId = vehicle.VehicleId, VehicleServiceId = vs.Id, Cost = 100, Description = "Oil", ServiceDate = DateTime.UtcNow, CreatedById = "user1", CreatedOn = DateTime.UtcNow };
            dbContext.ServiceRecords.Add(sr);
            await dbContext.SaveChangesAsync();

            var dto = new ServiceRecordDto { Id = sr.ServiceRecordId, VehicleId = vehicle.VehicleId, VehicleServiceId = vs.Id, Cost = 300, Description = "Full Service", ServiceDate = DateTime.UtcNow };

            var result = await service.UpdateServiceRecordAsync(dto);

            Assert.That(result, Is.True);
            var updated = await dbContext.ServiceRecords.FindAsync(sr.ServiceRecordId);
            Assert.That(updated!.Cost, Is.EqualTo(300));
        }

        [Test]
        public async Task UpdateServiceRecordAsync_WhenNotFound_ReturnsFalse()
        {
            var dto = new ServiceRecordDto { Id = 999, VehicleId = 1, VehicleServiceId = 1, Cost = 100, Description = "Test", ServiceDate = DateTime.UtcNow };

            var result = await service.UpdateServiceRecordAsync(dto);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteServiceRecordAsync_WhenExists_DeletesAndReturnsTrue()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            var sr = new ServiceRecord { VehicleId = vehicle.VehicleId, VehicleServiceId = vs.Id, Cost = 100, Description = "Oil", ServiceDate = DateTime.UtcNow, CreatedById = "user1", CreatedOn = DateTime.UtcNow };
            dbContext.ServiceRecords.Add(sr);
            await dbContext.SaveChangesAsync();

            var result = await service.DeleteServiceRecordAsync(sr.ServiceRecordId);

            Assert.That(result, Is.True);
            Assert.That(await dbContext.ServiceRecords.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteServiceRecordAsync_WhenNotFound_ReturnsFalse()
        {
            var result = await service.DeleteServiceRecordAsync(999);

            Assert.That(result, Is.False);
        }
    }
}
