using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VMAPP.Data;
using VMAPP.Data.Models;
using VMAPP.Data.Models.Enums;
using VMAPP.Services;
using VMAPP.Services.DTOs.VehicleServiceDTOs;

namespace VMAPP.Test.Services
{
    [TestFixture]
    public class VSManagementServiceTests
    {
        private ApplicationDbContext dbContext = null!;
        private VSManagementService service = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            dbContext = new ApplicationDbContext(options);
            service = new VSManagementService(dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        [Test]
        public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
        {
            var result = await service.GetAllAsync();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllAsync_ReturnsServicesOrderedByNameThenCreatedOn()
        {
            dbContext.VehicleServices.AddRange(
                new VehicleService { Name = "SpeedShop", Description = "Tuning", City = "Plovdiv", Address = "Str 2", Email = "b@b.com", Phone = "0888111111", CreatedOn = DateTime.UtcNow },
                new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow }
            );
            await dbContext.SaveChangesAsync();

            var result = await service.GetAllAsync();

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("AutoFix"));
            Assert.That(result[1].Name, Is.EqualTo("SpeedShop"));
        }

        [Test]
        public async Task GetByIdAsync_WhenExists_ReturnsDto()
        {
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            var result = await service.GetByIdAsync(vs.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("AutoFix"));
        }

        [Test]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            var result = await service.GetByIdAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateAsync_AddsServiceAndReturnsId()
        {
            var dto = new VehicleServiceDto
            {
                Name = "AutoFix",
                Description = "Repairs",
                City = "Sofia",
                Address = "Str 1",
                Email = "a@a.com",
                Phone = "0888000000"
            };

            var id = await service.CreateAsync(dto);

            Assert.That(id, Is.GreaterThan(0));
            Assert.That(await dbContext.VehicleServices.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateAsync_WhenExists_UpdatesFieldsAndReturnsTrue()
        {
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            var dto = new VehicleServiceDto { Id = vs.Id, Name = "SpeedShop", Description = "Tuning", City = "Plovdiv", Address = "Str 2", Email = "b@b.com", Phone = "0888111111" };

            var result = await service.UpdateAsync(dto);

            Assert.That(result, Is.True);
            var updated = await dbContext.VehicleServices.FindAsync(vs.Id);
            Assert.That(updated!.Name, Is.EqualTo("SpeedShop"));
        }

        [Test]
        public async Task UpdateAsync_WhenNotFound_ReturnsFalse()
        {
            var dto = new VehicleServiceDto { Id = 999, Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000" };

            var result = await service.UpdateAsync(dto);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteAsync_WhenExists_DeletesAndReturnsTrue()
        {
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            var result = await service.DeleteAsync(vs.Id);

            Assert.That(result, Is.True);
            Assert.That(await dbContext.VehicleServices.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteAsync_WhenNotFound_ReturnsFalse()
        {
            var result = await service.DeleteAsync(999);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ExistsAsync_WhenExists_ReturnsTrue()
        {
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            var result = await service.ExistsAsync(vs.Id);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsAsync_WhenNotFound_ReturnsFalse()
        {
            var result = await service.ExistsAsync(999);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetVehiclesByServiceIdAsync_ReturnsVehiclesForService()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            dbContext.ServiceRecords.Add(new ServiceRecord { VehicleId = vehicle.VehicleId, VehicleServiceId = vs.Id, Cost = 0, Description = "Init", ServiceDate = DateTime.UtcNow, CreatedById = "user1", CreatedOn = DateTime.UtcNow });
            await dbContext.SaveChangesAsync();

            var result = await service.GetVehiclesByServiceIdAsync(vs.Id);

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].CarBrand, Is.EqualTo("Toyota"));
        }

        [Test]
        public async Task GetVehiclesServiceDetailsByIdAsync_WhenNotFound_ReturnsNull()
        {
            var result = await service.GetVehiclesServiceDetailsByIdAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetVehiclesServiceDetailsByIdAsync_WhenExists_ReturnsDetailsWithVehicles()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            dbContext.ServiceRecords.Add(new ServiceRecord { VehicleId = vehicle.VehicleId, VehicleServiceId = vs.Id, Cost = 0, Description = "Init", ServiceDate = DateTime.UtcNow, CreatedById = "user1", CreatedOn = DateTime.UtcNow });
            await dbContext.SaveChangesAsync();

            var result = await service.GetVehiclesServiceDetailsByIdAsync(vs.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("AutoFix"));
            Assert.That(result.Vehicles, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task GetVehicleByVinAsync_WhenExists_ReturnsVehicleDto()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            await dbContext.SaveChangesAsync();

            var result = await service.GetVehicleByVinAsync("11111111111111111");

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.CarBrand, Is.EqualTo("Toyota"));
        }

        [Test]
        public async Task GetVehicleByVinAsync_WhenNotFound_ReturnsNull()
        {
            var result = await service.GetVehicleByVinAsync("NONEXISTENTVINXX");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task AddVehicleToServiceAsync_WhenNotAlreadyAssigned_AddsRecordAndReturnsTrue()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            var result = await service.AddVehicleToServiceAsync(vs.Id, vehicle.VehicleId, "user1");

            Assert.That(result, Is.True);
            Assert.That(await dbContext.ServiceRecords.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task AddVehicleToServiceAsync_WhenAlreadyAssigned_ReturnsFalse()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            dbContext.ServiceRecords.Add(new ServiceRecord { VehicleId = vehicle.VehicleId, VehicleServiceId = vs.Id, Cost = 0, Description = "Init", ServiceDate = DateTime.UtcNow, CreatedById = "user1", CreatedOn = DateTime.UtcNow });
            await dbContext.SaveChangesAsync();

            var result = await service.AddVehicleToServiceAsync(vs.Id, vehicle.VehicleId, "user1");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task RemoveVehicleFromServiceAsync_WhenNotAssigned_ReturnsFalseWithMessage()
        {
            var result = await service.RemoveVehicleFromServiceAsync(1, 1);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.Not.Null);
        }

        [Test]
        public async Task RemoveVehicleFromServiceAsync_WhenHasPaidRecords_ReturnsFalseWithMessage()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            dbContext.ServiceRecords.Add(new ServiceRecord { VehicleId = vehicle.VehicleId, VehicleServiceId = vs.Id, Cost = 150, Description = "Paid service", ServiceDate = DateTime.UtcNow, CreatedById = "user1", CreatedOn = DateTime.UtcNow });
            await dbContext.SaveChangesAsync();

            var result = await service.RemoveVehicleFromServiceAsync(vs.Id, vehicle.VehicleId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.Not.Null);
        }

        [Test]
        public async Task RemoveVehicleFromServiceAsync_WhenNoPaidRecords_RemovesAndReturnsSuccess()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var vs = new VehicleService { Name = "AutoFix", Description = "Repairs", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            dbContext.VehicleServices.Add(vs);
            await dbContext.SaveChangesAsync();

            dbContext.ServiceRecords.Add(new ServiceRecord { VehicleId = vehicle.VehicleId, VehicleServiceId = vs.Id, Cost = 0, Description = "Free check", ServiceDate = DateTime.UtcNow, CreatedById = "user1", CreatedOn = DateTime.UtcNow });
            await dbContext.SaveChangesAsync();

            var result = await service.RemoveVehicleFromServiceAsync(vs.Id, vehicle.VehicleId);

            Assert.That(result.Success, Is.True);
            Assert.That(await dbContext.ServiceRecords.CountAsync(), Is.EqualTo(0));
        }
    }
}
