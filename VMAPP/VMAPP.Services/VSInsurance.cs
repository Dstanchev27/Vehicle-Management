using Microsoft.EntityFrameworkCore;

using VMAPP.Data;
using VMAPP.Data.Models;
using VMAPP.Services.DTOs.InsuranceDTOs;
using VMAPP.Services.DTOs.VehicleDTOs;
using VMAPP.Services.Interfaces;

namespace VMAPP.Services
{
    public class VSInsurance : IVSInsuranceService
    {
        private readonly ApplicationDbContext dbContext;

        public VSInsurance(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IReadOnlyList<InsuranceCompanyDto>> GetAllAsync()
        {
            return await this.dbContext.InsuranceCompanies
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new InsuranceCompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedOn = c.CreatedOn,
                    City = c.City,
                    Address = c.Address,
                    Email = c.Email,
                    Phone = c.Phone
                })
                .ToListAsync();
        }

        public async Task<InsuranceCompanyDto?> GetByIdAsync(int id)
        {
            var entity = await this.dbContext.InsuranceCompanies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                return null;
            }

            return new InsuranceCompanyDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CreatedOn = entity.CreatedOn,
                City = entity.City,
                Address = entity.Address,
                Email = entity.Email,
                Phone = entity.Phone
            };
        }

        public async Task<int> CreateAsync(InsuranceCompanyDto dto)
        {
            var createdById = dto.CreatedById;

            if (string.IsNullOrEmpty(createdById))
            {
                createdById = await this.dbContext.Users
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();
            }

            if (createdById == null)
            {
                throw new InvalidOperationException("No user found to assign as creator.");
            }

            var entity = new InsuranceCompany
            {
                Name = dto.Name,
                Description = dto.Description,
                City = dto.City,
                Address = dto.Address,
                Email = dto.Email,
                Phone = dto.Phone,
                CreatedOn = dto.CreatedOn == default ? DateTime.UtcNow : dto.CreatedOn,
                CreatedById = createdById
            };

            await this.dbContext.InsuranceCompanies.AddAsync(entity);
            await this.dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(InsuranceCompanyDto dto)
        {
            var entity = await this.dbContext.InsuranceCompanies
                .FirstOrDefaultAsync(c => c.Id == dto.Id);

            if (entity == null)
            {
                return false;
            }

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.City = dto.City;
            entity.Address = dto.Address;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.ModifiedOn = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(dto.ModifiedById))
            {
                entity.ModifiedById = dto.ModifiedById;
            }

            this.dbContext.InsuranceCompanies.Update(entity);
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await this.dbContext.InsuranceCompanies
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                return false;
            }

            this.dbContext.InsuranceCompanies.Remove(entity);
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<InsuranceCompanyWithVehiclesDto?> GetCompanyWithVehiclesAsync(int id)
        {
            var entity = await this.dbContext.InsuranceCompanies
                .AsNoTracking()
                .Include(c => c.InsurancePolicies.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.Vehicle)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                return null;
            }

            return new InsuranceCompanyWithVehiclesDto
            {
                Id = entity.Id,
                Name = entity.Name,
                City = entity.City,
                Address = entity.Address,
                Email = entity.Email,
                Phone = entity.Phone,
                Description = entity.Description,
                CreatedOn = entity.CreatedOn,
                Vehicles = entity.InsurancePolicies
                    .Select(p => new VehicleWithPolicyIdDto
                    {
                        Id = p.Vehicle.VehicleId,
                        PolicyId = p.Id,
                        VIN = p.Vehicle.VIN,
                        CarBrand = p.Vehicle.CarBrand,
                        CarModel = p.Vehicle.CarModel,
                        CreatedOnYear = p.Vehicle.CreatedOnYear,
                        Color = p.Vehicle.Color,
                        VehicleType = p.Vehicle.VehicleType,
                        PolicyNumber = p.PolicyNumber
                    })
                    .ToList()
            };
        }

        public async Task<InsurancePolicyDetailsDto?> GetPolicyDetailsAsync(int id)
        {
            var entity = await this.dbContext.InsurancePolicies
                .AsNoTracking()
                .Include(p => p.Vehicle)
                .Include(p => p.InsuranceCompany)
                .Include(p => p.Claims.Where(c => !c.IsDeleted))
                .FirstOrDefaultAsync(p => p.Id == id);

            if (entity == null)
            {
                return null;
            }

            return new InsurancePolicyDetailsDto
            {
                Id = entity.Id,
                PolicyNumber = entity.PolicyNumber,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                VehicleId = entity.VehicleId,
                VehicleVIN = entity.Vehicle.VIN,
                VehicleBrand = entity.Vehicle.CarBrand,
                VehicleModel = entity.Vehicle.CarModel,
                InsuranceCompanyId = entity.InsuranceCompanyId,
                InsuranceCompanyName = entity.InsuranceCompany.Name,
                Claims = entity.Claims
                    .Select(c => new InsuranceClaimDto
                    {
                        Id = c.Id,
                        ClaimDate = c.ClaimDate,
                        Description = c.Description,
                        Amount = c.Amount
                    })
                    .ToList()
            };
        }

        public async Task<VehicleDto?> GetVehicleByVinAsync(string vin)
        {
            var vehicle = await this.dbContext.Vehicles
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.VIN == vin);

            if (vehicle == null)
            {
                return null;
            }

            return new VehicleDto
            {
                Id = vehicle.VehicleId,
                VIN = vehicle.VIN,
                CarBrand = vehicle.CarBrand,
                CarModel = vehicle.CarModel,
                CreatedOnYear = vehicle.CreatedOnYear,
                Color = vehicle.Color,
                VehicleType = vehicle.VehicleType
            };
        }

        public async Task<int> AddPolicyAsync(InsurancePolicyFormDto dto)
        {
            var createdById = dto.CreatedById;
            if (string.IsNullOrEmpty(createdById))
            {
                createdById = await this.dbContext.Users
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();
            }

            var entity = new InsurancePolicy
            {
                VehicleId = dto.VehicleId,
                InsuranceCompanyId = dto.InsuranceCompanyId,
                PolicyNumber = dto.PolicyNumber,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CreatedOn = DateTime.UtcNow,
                CreatedById = createdById!
            };

            await this.dbContext.InsurancePolicies.AddAsync(entity);
            await this.dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> DeletePolicyAsync(int id)
        {
            var entity = await this.dbContext.InsurancePolicies
                .FirstOrDefaultAsync(p => p.Id == id);

            if (entity == null)
            {
                return false;
            }

            this.dbContext.InsurancePolicies.Remove(entity);
            await this.dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<InsuranceClaimFormDto?> GetClaimByIdAsync(int id)
        {
            var entity = await this.dbContext.InsuranceClaims
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                return null;
            }

            return new InsuranceClaimFormDto
            {
                Id = entity.Id,
                InsurancePolicyId = entity.InsurancePolicyId,
                ClaimDate = entity.ClaimDate,
                Description = entity.Description,
                Amount = entity.Amount
            };
        }

        public async Task<int> AddClaimAsync(InsuranceClaimFormDto dto)
        {
            var entity = new InsuranceClaim
            {
                InsurancePolicyId = dto.InsurancePolicyId,
                ClaimDate = dto.ClaimDate,
                Description = dto.Description,
                Amount = dto.Amount,
                CreatedOn = DateTime.UtcNow
            };

            await this.dbContext.InsuranceClaims.AddAsync(entity);
            await this.dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> DeleteClaimAsync(int id)
        {
            var entity = await this.dbContext.InsuranceClaims
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                return false;
            }

            this.dbContext.InsuranceClaims.Remove(entity);
            await this.dbContext.SaveChangesAsync();
            return true;
        }
    }
}

