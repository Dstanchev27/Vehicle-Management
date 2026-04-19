using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VMAPP.Data;
using VMAPP.Data.Models;
using VMAPP.Data.Models.Enums;
using VMAPP.Services;
using VMAPP.Services.DTOs.AnnualReviewDTOs;

namespace VMAPP.Test.Services
{
    [TestFixture]
    public class VSAnnualReviewServiceTests
    {
        private ApplicationDbContext dbContext = null!;
        private VSAnnualReview service = null!;

        private const string TestUserId = "test-user-id";

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            dbContext = new ApplicationDbContext(options);
            service = new VSAnnualReview(dbContext);
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
        public async Task GetAllAsync_ReturnsCompaniesOrderedByName()
        {
            dbContext.AnnualReviewCompanies.AddRange(
                new AnnualReviewCompany { Name = "Zurich", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "z@z.com", Phone = "0888000001", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId },
                new AnnualReviewCompany { Name = "Allianz", Description = "Desc", City = "Sofia", Address = "Str 2", Email = "a@a.com", Phone = "0888000002", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId }
            );
            await dbContext.SaveChangesAsync();

            var result = await service.GetAllAsync();

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Allianz"));
            Assert.That(result[1].Name, Is.EqualTo("Zurich"));
        }


        [Test]
        public async Task GetByIdAsync_WhenExists_ReturnsDto()
        {
            var company = new AnnualReviewCompany { Name = "TechCheck", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "t@t.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.AnnualReviewCompanies.Add(company);
            await dbContext.SaveChangesAsync();

            var result = await service.GetByIdAsync(company.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("TechCheck"));
        }

        [Test]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            var result = await service.GetByIdAsync(999);

            Assert.That(result, Is.Null);
        }


        [Test]
        public async Task CreateAsync_WithCreatedById_CreatesCompanyAndReturnsId()
        {
            var dto = new AnnualReviewCompanyDto
            {
                Name = "TechCheck",
                Description = "Annual review services",
                City = "Sofia",
                Address = "Str 1",
                Email = "t@t.com",
                Phone = "0888000000",
                CreatedById = TestUserId
            };

            var id = await service.CreateAsync(dto);

            Assert.That(id, Is.GreaterThan(0));
            Assert.That(await dbContext.AnnualReviewCompanies.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public void CreateAsync_WhenNoCreatedByIdAndNoUsers_ThrowsInvalidOperationException()
        {
            var dto = new AnnualReviewCompanyDto
            {
                Name = "TechCheck",
                Description = "Annual review services",
                City = "Sofia",
                Address = "Str 1",
                Email = "t@t.com",
                Phone = "0888000000"
            };

            Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(dto));
        }


        [Test]
        public async Task UpdateAsync_WhenExists_UpdatesFieldsAndReturnsTrue()
        {
            var company = new AnnualReviewCompany { Name = "TechCheck", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "t@t.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.AnnualReviewCompanies.Add(company);
            await dbContext.SaveChangesAsync();

            var dto = new AnnualReviewCompanyDto { Id = company.Id, Name = "AutoInspect", Description = "New Desc", City = "Plovdiv", Address = "Str 2", Email = "a@a.com", Phone = "0888111111" };

            var result = await service.UpdateAsync(dto);

            Assert.That(result, Is.True);
            var updated = await dbContext.AnnualReviewCompanies.FindAsync(company.Id);
            Assert.That(updated!.Name, Is.EqualTo("AutoInspect"));
            Assert.That(updated.City, Is.EqualTo("Plovdiv"));
        }

        [Test]
        public async Task UpdateAsync_WhenNotFound_ReturnsFalse()
        {
            var dto = new AnnualReviewCompanyDto { Id = 999, Name = "AutoInspect", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000" };

            var result = await service.UpdateAsync(dto);

            Assert.That(result, Is.False);
        }


        [Test]
        public async Task DeleteAsync_WhenExists_DeletesAndReturnsTrue()
        {
            var company = new AnnualReviewCompany { Name = "TechCheck", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "t@t.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.AnnualReviewCompanies.Add(company);
            await dbContext.SaveChangesAsync();

            var result = await service.DeleteAsync(company.Id);

            Assert.That(result, Is.True);
            Assert.That(await dbContext.AnnualReviewCompanies.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteAsync_WhenNotFound_ReturnsFalse()
        {
            var result = await service.DeleteAsync(999);

            Assert.That(result, Is.False);
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
        public async Task GetCompanyWithVehiclesAsync_WhenNotFound_ReturnsNull()
        {
            var result = await service.GetCompanyWithVehiclesAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCompanyWithVehiclesAsync_WhenExists_ReturnsCompanyWithVehicles()
        {
            var company = new AnnualReviewCompany { Name = "TechCheck", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "t@t.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.AnnualReviewCompanies.Add(company);
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            await dbContext.SaveChangesAsync();

            var report = new AnnualReport
            {
                VehicleId = vehicle.VehicleId,
                AnnualReviewCompanyId = company.Id,
                ReportNumber = "RPT-001",
                InspectionDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                Passed = true,
                CreatedOn = DateTime.UtcNow,
                CreatedById = TestUserId
            };
            dbContext.AnnualReports.Add(report);
            await dbContext.SaveChangesAsync();

            var result = await service.GetCompanyWithVehiclesAsync(company.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("TechCheck"));
            Assert.That(result.Vehicles, Has.Count.EqualTo(1));
            Assert.That(result.Vehicles[0].VIN, Is.EqualTo("11111111111111111"));
        }

        [Test]
        public async Task GetReportDetailsAsync_WhenNotFound_ReturnsNull()
        {
            var result = await service.GetReportDetailsAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetReportDetailsAsync_WhenExists_ReturnsDtoWithDetails()
        {
            var company = new AnnualReviewCompany { Name = "TechCheck", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "t@t.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.AnnualReviewCompanies.Add(company);
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            await dbContext.SaveChangesAsync();

            var report = new AnnualReport
            {
                VehicleId = vehicle.VehicleId,
                AnnualReviewCompanyId = company.Id,
                ReportNumber = "RPT-001",
                InspectionDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                Passed = true,
                Notes = "All good",
                CreatedOn = DateTime.UtcNow,
                CreatedById = TestUserId
            };
            dbContext.AnnualReports.Add(report);
            await dbContext.SaveChangesAsync();

            var result = await service.GetReportDetailsAsync(report.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ReportNumber, Is.EqualTo("RPT-001"));
            Assert.That(result.VehicleVIN, Is.EqualTo("11111111111111111"));
            Assert.That(result.AnnualReviewCompanyName, Is.EqualTo("TechCheck"));
            Assert.That(result.Passed, Is.True);
        }


        [Test]
        public async Task AddReportAsync_WithCreatedById_AddsReportAndReturnsId()
        {
            var company = new AnnualReviewCompany { Name = "TechCheck", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "t@t.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.AnnualReviewCompanies.Add(company);
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            await dbContext.SaveChangesAsync();

            var dto = new AnnualReportFormDto
            {
                VehicleId = vehicle.VehicleId,
                AnnualReviewCompanyId = company.Id,
                ReportNumber = "RPT-001",
                InspectionDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                Passed = true,
                Notes = "All good",
                CreatedById = TestUserId
            };

            var id = await service.AddReportAsync(dto);

            Assert.That(id, Is.GreaterThan(0));
            Assert.That(await dbContext.AnnualReports.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteReportAsync_WhenExists_DeletesAndReturnsTrue()
        {
            var company = new AnnualReviewCompany { Name = "TechCheck", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "t@t.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.AnnualReviewCompanies.Add(company);
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            await dbContext.SaveChangesAsync();

            var report = new AnnualReport
            {
                VehicleId = vehicle.VehicleId,
                AnnualReviewCompanyId = company.Id,
                ReportNumber = "RPT-001",
                InspectionDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                CreatedOn = DateTime.UtcNow,
                CreatedById = TestUserId
            };
            dbContext.AnnualReports.Add(report);
            await dbContext.SaveChangesAsync();

            var result = await service.DeleteReportAsync(report.Id);

            Assert.That(result, Is.True);
            Assert.That(await dbContext.AnnualReports.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteReportAsync_WhenNotFound_ReturnsFalse()
        {
            var result = await service.DeleteReportAsync(999);

            Assert.That(result, Is.False);
        }


        [Test]
        public async Task AssignUserAsync_WhenUserNotFound_ReturnsFalse()
        {
            var result = await service.AssignUserAsync("nonexistent-user", null);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AssignUserAsync_WhenCompanyNotFound_ReturnsFalse()
        {
            var user = new ApplicationUser { Id = TestUserId, UserName = "testuser", Email = "u@u.com", City = "Sofia", Address = "Str 1", CreatedOn = DateTime.UtcNow };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var result = await service.AssignUserAsync(TestUserId, 999);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AssignUserAsync_WhenUserExistsAndNoCompany_AssignsNullAndReturnsTrue()
        {
            var user = new ApplicationUser { Id = TestUserId, UserName = "testuser", Email = "u@u.com", City = "Sofia", Address = "Str 1", CreatedOn = DateTime.UtcNow };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var result = await service.AssignUserAsync(TestUserId, null);

            Assert.That(result, Is.True);
            var updated = await dbContext.Users.FindAsync(TestUserId);
            Assert.That(updated!.AnnualReviewCompanyId, Is.Null);
        }

        [Test]
        public async Task AssignUserAsync_WhenUserAndCompanyExist_AssignsCompanyAndReturnsTrue()
        {
            var user = new ApplicationUser { Id = TestUserId, UserName = "testuser", Email = "u@u.com", City = "Sofia", Address = "Str 1", CreatedOn = DateTime.UtcNow };
            dbContext.Users.Add(user);
            var company = new AnnualReviewCompany { Name = "TechCheck", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "t@t.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.AnnualReviewCompanies.Add(company);
            await dbContext.SaveChangesAsync();

            var result = await service.AssignUserAsync(TestUserId, company.Id);

            Assert.That(result, Is.True);
            var updated = await dbContext.Users.FindAsync(TestUserId);
            Assert.That(updated!.AnnualReviewCompanyId, Is.EqualTo(company.Id));
        }


        [Test]
        public async Task GetCompanyIdByReportIdAsync_WhenReportNotFound_ReturnsNull()
        {
            var result = await service.GetCompanyIdByReportIdAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCompanyIdByReportIdAsync_WhenExists_ReturnsCorrectCompanyId()
        {
            var company = new AnnualReviewCompany { Name = "TechCheck", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "t@t.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.AnnualReviewCompanies.Add(company);
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            dbContext.Vehicles.Add(vehicle);
            await dbContext.SaveChangesAsync();

            var report = new AnnualReport
            {
                VehicleId = vehicle.VehicleId,
                AnnualReviewCompanyId = company.Id,
                InspectionDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                CreatedOn = DateTime.UtcNow,
                CreatedById = TestUserId
            };
            dbContext.AnnualReports.Add(report);
            await dbContext.SaveChangesAsync();

            var result = await service.GetCompanyIdByReportIdAsync(report.Id);

            Assert.That(result, Is.EqualTo(company.Id));
        }
    }
}