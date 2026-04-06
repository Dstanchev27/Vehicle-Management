using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VMAPP.Data;
using VMAPP.Data.Models;
using VMAPP.Data.Models.Enums;
using VMAPP.Services;
using VMAPP.Services.DTOs.VehicleDTOs;

namespace VMAPP.Test.Services
{
    [TestFixture]
    public class VSServiceTests
    {
        private ApplicationDbContext dbContext = null!;
        private VSService service = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            dbContext = new ApplicationDbContext(options);
            service = new VSService(dbContext);
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
        public async Task GetAllAsync_ReturnsVehiclesOrderedByBrandThenModel()
        {
            dbContext.Vehicles.AddRange(
                new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow },
                new Vehicle { VIN = "22222222222222222", CarBrand = "BMW", CarModel = "X5", CreatedOnYear = 2021, Color = "Blue", VehicleType = VehicleType.SUV, CreatedOn = DateTime.UtcNow }
            );
            await dbContext.SaveChangesAsync();

            var result = await service.GetAllAsync();

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].CarBrand, Is.EqualTo("BMW"));
            Assert.That(result[1].CarBrand, Is.EqualTo("Toyota"));
        }

        [Test]
        public async Task GetByIdAsync_WhenVehicleExists_ReturnsVehicleDto()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            await dbContext.SaveChangesAsync();

            var result = await service.GetByIdAsync(vehicle.VehicleId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.CarBrand, Is.EqualTo("Toyota"));
            Assert.That(result.VIN, Is.EqualTo("11111111111111111"));
        }

        [Test]
        public async Task GetByIdAsync_WhenVehicleNotFound_ReturnsNull()
        {
            var result = await service.GetByIdAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateAsync_AddsVehicleAndReturnsNewId()
        {
            var dto = new VehicleDto
            {
                VIN = "11111111111111111",
                CarBrand = "Toyota",
                CarModel = "Camry",
                CreatedOnYear = 2020,
                Color = "Red",
                VehicleType = VehicleType.Sedan
            };

            var id = await service.CreateAsync(dto);

            Assert.That(id, Is.GreaterThan(0));
            Assert.That(await dbContext.Vehicles.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateAsync_WhenVehicleExists_UpdatesFieldsAndReturnsTrue()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            await dbContext.SaveChangesAsync();

            var dto = new VehicleDto
            {
                Id = vehicle.VehicleId,
                VIN = "22222222222222222",
                CarBrand = "BMW",
                CarModel = "X5",
                CreatedOnYear = 2021,
                Color = "Blue",
                VehicleType = VehicleType.SUV
            };

            var result = await service.UpdateAsync(dto);

            Assert.That(result, Is.True);
            var updated = await dbContext.Vehicles.FindAsync(vehicle.VehicleId);
            Assert.That(updated!.CarBrand, Is.EqualTo("BMW"));
            Assert.That(updated.VIN, Is.EqualTo("22222222222222222"));
        }

        [Test]
        public async Task UpdateAsync_WhenVehicleNotFound_ReturnsFalse()
        {
            var dto = new VehicleDto { Id = 999, VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red" };

            var result = await service.UpdateAsync(dto);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteAsync_WhenVehicleExists_DeletesAndReturnsTrue()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            await dbContext.SaveChangesAsync();

            var result = await service.DeleteAsync(vehicle.VehicleId);

            Assert.That(result, Is.True);
            Assert.That(await dbContext.Vehicles.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteAsync_WhenVehicleNotFound_ReturnsFalse()
        {
            var result = await service.DeleteAsync(999);

            Assert.That(result, Is.False);
        }
    }
}
