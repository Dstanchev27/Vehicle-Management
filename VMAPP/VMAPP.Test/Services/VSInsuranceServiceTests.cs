using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VMAPP.Data;
using VMAPP.Data.Models;
using VMAPP.Data.Models.Enums;
using VMAPP.Services;
using VMAPP.Services.DTOs.InsuranceDTOs;

namespace VMAPP.Test.Services
{
    [TestFixture]
    public class VSInsuranceServiceTests
    {
        private ApplicationDbContext dbContext = null!;
        private VSInsurance service = null!;

        private const string TestUserId = "test-user-id";

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            dbContext = new ApplicationDbContext(options);
            service = new VSInsurance(dbContext);
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
            dbContext.InsuranceCompanies.AddRange(
                new InsuranceCompany { Name = "Zurich", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "z@z.com", Phone = "0888000001", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId },
                new InsuranceCompany { Name = "Allianz", Description = "Desc", City = "Sofia", Address = "Str 2", Email = "a@a.com", Phone = "0888000002", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId }
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
            var company = new InsuranceCompany { Name = "Allianz", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.InsuranceCompanies.Add(company);
            await dbContext.SaveChangesAsync();

            var result = await service.GetByIdAsync(company.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Allianz"));
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
            var dto = new InsuranceCompanyDto
            {
                Name = "Allianz",
                Description = "Insurance",
                City = "Sofia",
                Address = "Str 1",
                Email = "a@a.com",
                Phone = "0888000000",
                CreatedById = TestUserId
            };

            var id = await service.CreateAsync(dto);

            Assert.That(id, Is.GreaterThan(0));
            Assert.That(await dbContext.InsuranceCompanies.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public void CreateAsync_WhenNoCreatedByIdAndNoUsers_ThrowsInvalidOperationException()
        {
            var dto = new InsuranceCompanyDto
            {
                Name = "Allianz",
                Description = "Insurance",
                City = "Sofia",
                Address = "Str 1",
                Email = "a@a.com",
                Phone = "0888000000"
            };

            Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(dto));
        }

        [Test]
        public async Task UpdateAsync_WhenExists_UpdatesFieldsAndReturnsTrue()
        {
            var company = new InsuranceCompany { Name = "Allianz", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.InsuranceCompanies.Add(company);
            await dbContext.SaveChangesAsync();

            var dto = new InsuranceCompanyDto { Id = company.Id, Name = "Zurich", Description = "New Desc", City = "Plovdiv", Address = "Str 2", Email = "z@z.com", Phone = "0888111111" };

            var result = await service.UpdateAsync(dto);

            Assert.That(result, Is.True);
            var updated = await dbContext.InsuranceCompanies.FindAsync(company.Id);
            Assert.That(updated!.Name, Is.EqualTo("Zurich"));
        }

        [Test]
        public async Task UpdateAsync_WhenNotFound_ReturnsFalse()
        {
            var dto = new InsuranceCompanyDto { Id = 999, Name = "Zurich", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "z@z.com", Phone = "0888000000" };

            var result = await service.UpdateAsync(dto);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteAsync_WhenExists_DeletesAndReturnsTrue()
        {
            var company = new InsuranceCompany { Name = "Allianz", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.InsuranceCompanies.Add(company);
            await dbContext.SaveChangesAsync();

            var result = await service.DeleteAsync(company.Id);

            Assert.That(result, Is.True);
            Assert.That(await dbContext.InsuranceCompanies.CountAsync(), Is.EqualTo(0));
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
        public async Task AddPolicyAsync_WithCreatedById_AddsPolicyAndReturnsId()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var company = new InsuranceCompany { Name = "Allianz", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.Vehicles.Add(vehicle);
            dbContext.InsuranceCompanies.Add(company);
            await dbContext.SaveChangesAsync();

            var dto = new InsurancePolicyFormDto
            {
                VehicleId = vehicle.VehicleId,
                InsuranceCompanyId = company.Id,
                PolicyNumber = "POL-001",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddYears(1),
                CreatedById = TestUserId
            };

            var id = await service.AddPolicyAsync(dto);

            Assert.That(id, Is.GreaterThan(0));
            Assert.That(await dbContext.InsurancePolicies.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task DeletePolicyAsync_WhenExists_DeletesAndReturnsTrue()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var company = new InsuranceCompany { Name = "Allianz", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.Vehicles.Add(vehicle);
            dbContext.InsuranceCompanies.Add(company);
            await dbContext.SaveChangesAsync();

            var policy = new InsurancePolicy { VehicleId = vehicle.VehicleId, InsuranceCompanyId = company.Id, PolicyNumber = "POL-001", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddYears(1), CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.InsurancePolicies.Add(policy);
            await dbContext.SaveChangesAsync();

            var result = await service.DeletePolicyAsync(policy.Id);

            Assert.That(result, Is.True);
            Assert.That(await dbContext.InsurancePolicies.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeletePolicyAsync_WhenNotFound_ReturnsFalse()
        {
            var result = await service.DeletePolicyAsync(999);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetClaimByIdAsync_WhenExists_ReturnsDto()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var company = new InsuranceCompany { Name = "Allianz", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.Vehicles.Add(vehicle);
            dbContext.InsuranceCompanies.Add(company);
            await dbContext.SaveChangesAsync();

            var policy = new InsurancePolicy { VehicleId = vehicle.VehicleId, InsuranceCompanyId = company.Id, PolicyNumber = "POL-001", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddYears(1), CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.InsurancePolicies.Add(policy);
            await dbContext.SaveChangesAsync();

            var claim = new InsuranceClaim { InsurancePolicyId = policy.Id, ClaimDate = DateTime.UtcNow, Description = "Accident", Amount = 500, CreatedOn = DateTime.UtcNow };
            dbContext.InsuranceClaims.Add(claim);
            await dbContext.SaveChangesAsync();

            var result = await service.GetClaimByIdAsync(claim.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Amount, Is.EqualTo(500));
        }

        [Test]
        public async Task GetClaimByIdAsync_WhenNotFound_ReturnsNull()
        {
            var result = await service.GetClaimByIdAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task AddClaimAsync_AddsClaimAndReturnsId()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var company = new InsuranceCompany { Name = "Allianz", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.Vehicles.Add(vehicle);
            dbContext.InsuranceCompanies.Add(company);
            await dbContext.SaveChangesAsync();

            var policy = new InsurancePolicy { VehicleId = vehicle.VehicleId, InsuranceCompanyId = company.Id, PolicyNumber = "POL-001", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddYears(1), CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.InsurancePolicies.Add(policy);
            await dbContext.SaveChangesAsync();

            var dto = new InsuranceClaimFormDto { InsurancePolicyId = policy.Id, ClaimDate = DateTime.UtcNow, Description = "Theft", Amount = 1000 };

            var id = await service.AddClaimAsync(dto);

            Assert.That(id, Is.GreaterThan(0));
            Assert.That(await dbContext.InsuranceClaims.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteClaimAsync_WhenExists_DeletesAndReturnsTrue()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var company = new InsuranceCompany { Name = "Allianz", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.Vehicles.Add(vehicle);
            dbContext.InsuranceCompanies.Add(company);
            await dbContext.SaveChangesAsync();

            var policy = new InsurancePolicy { VehicleId = vehicle.VehicleId, InsuranceCompanyId = company.Id, PolicyNumber = "POL-001", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddYears(1), CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.InsurancePolicies.Add(policy);
            await dbContext.SaveChangesAsync();

            var claim = new InsuranceClaim { InsurancePolicyId = policy.Id, ClaimDate = DateTime.UtcNow, Description = "Accident", Amount = 500, CreatedOn = DateTime.UtcNow };
            dbContext.InsuranceClaims.Add(claim);
            await dbContext.SaveChangesAsync();

            var result = await service.DeleteClaimAsync(claim.Id);

            Assert.That(result, Is.True);
            Assert.That(await dbContext.InsuranceClaims.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteClaimAsync_WhenNotFound_ReturnsFalse()
        {
            var result = await service.DeleteClaimAsync(999);

            Assert.That(result, Is.False);
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
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var company = new InsuranceCompany { Name = "Allianz", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.Vehicles.Add(vehicle);
            dbContext.InsuranceCompanies.Add(company);
            await dbContext.SaveChangesAsync();

            var policy = new InsurancePolicy { VehicleId = vehicle.VehicleId, InsuranceCompanyId = company.Id, PolicyNumber = "POL-001", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddYears(1), CreatedOn = DateTime.UtcNow, CreatedById = TestUserId, IsDeleted = false };
            dbContext.InsurancePolicies.Add(policy);
            await dbContext.SaveChangesAsync();

            var result = await service.GetCompanyWithVehiclesAsync(company.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Allianz"));
            Assert.That(result.Vehicles, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task GetPolicyDetailsAsync_WhenNotFound_ReturnsNull()
        {
            var result = await service.GetPolicyDetailsAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetPolicyDetailsAsync_WhenExists_ReturnsPolicyWithVehicleAndCompanyInfo()
        {
            var vehicle = new Vehicle { VIN = "11111111111111111", CarBrand = "Toyota", CarModel = "Camry", CreatedOnYear = 2020, Color = "Red", VehicleType = VehicleType.Sedan, CreatedOn = DateTime.UtcNow };
            var company = new InsuranceCompany { Name = "Allianz", Description = "Desc", City = "Sofia", Address = "Str 1", Email = "a@a.com", Phone = "0888000000", CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.Vehicles.Add(vehicle);
            dbContext.InsuranceCompanies.Add(company);
            await dbContext.SaveChangesAsync();

            var policy = new InsurancePolicy { VehicleId = vehicle.VehicleId, InsuranceCompanyId = company.Id, PolicyNumber = "POL-001", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddYears(1), CreatedOn = DateTime.UtcNow, CreatedById = TestUserId };
            dbContext.InsurancePolicies.Add(policy);
            await dbContext.SaveChangesAsync();

            var result = await service.GetPolicyDetailsAsync(policy.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.PolicyNumber, Is.EqualTo("POL-001"));
            Assert.That(result.VehicleVIN, Is.EqualTo("11111111111111111"));
            Assert.That(result.InsuranceCompanyName, Is.EqualTo("Allianz"));
        }
    }
}
