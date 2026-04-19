using Microsoft.EntityFrameworkCore;

using VMAPP.Data;
using VMAPP.Data.Models;
using VMAPP.Services.DTOs.AnnualReviewDTOs;
using VMAPP.Services.DTOs.VehicleDTOs;
using VMAPP.Services.Interfaces;

namespace VMAPP.Services
{
    public class VSAnnualReview : IVSAnnualReviewService
    {
        private readonly ApplicationDbContext dbContext;

        public VSAnnualReview(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IReadOnlyList<AnnualReviewCompanyDto>> GetAllAsync()
        {
            return await this.dbContext.AnnualReviewCompanies
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new AnnualReviewCompanyDto
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

        public async Task<AnnualReviewCompanyDto?> GetByIdAsync(int id)
        {
            var entity = await this.dbContext.AnnualReviewCompanies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                return null;
            }

            return new AnnualReviewCompanyDto
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

        public async Task<int> CreateAsync(AnnualReviewCompanyDto dto)
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

            var entity = new AnnualReviewCompany
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

            await this.dbContext.AnnualReviewCompanies.AddAsync(entity);
            await this.dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(AnnualReviewCompanyDto dto)
        {
            var entity = await this.dbContext.AnnualReviewCompanies
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

            this.dbContext.AnnualReviewCompanies.Update(entity);
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await this.dbContext.AnnualReviewCompanies
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                return false;
            }

            this.dbContext.AnnualReviewCompanies.Remove(entity);
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<AnnualReviewCompanyWithVehiclesDto?> GetCompanyWithVehiclesAsync(int id)
        {
            var entity = await this.dbContext.AnnualReviewCompanies
                .AsNoTracking()
                .Include(c => c.AnnualReports.Where(r => !r.IsDeleted))
                    .ThenInclude(r => r.Vehicle)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                return null;
            }

            return new AnnualReviewCompanyWithVehiclesDto
            {
                Id = entity.Id,
                Name = entity.Name,
                City = entity.City,
                Address = entity.Address,
                Email = entity.Email,
                Phone = entity.Phone,
                Description = entity.Description,
                CreatedOn = entity.CreatedOn,
                Vehicles = entity.AnnualReports
                    .Select(r => new VehicleWithReportIdDto
                    {
                        Id = r.Vehicle.VehicleId,
                        ReportId = r.Id,
                        VIN = r.Vehicle.VIN,
                        CarBrand = r.Vehicle.CarBrand,
                        CarModel = r.Vehicle.CarModel,
                        CreatedOnYear = r.Vehicle.CreatedOnYear,
                        Color = r.Vehicle.Color,
                        VehicleType = r.Vehicle.VehicleType,
                        ReportNumber = r.ReportNumber
                    })
                    .ToList()
            };
        }

        public async Task<AnnualReportDetailsDto?> GetReportDetailsAsync(int id)
        {
            var entity = await this.dbContext.AnnualReports
                .AsNoTracking()
                .Include(r => r.Vehicle)
                .Include(r => r.AnnualReviewCompany)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (entity == null)
            {
                return null;
            }

            return new AnnualReportDetailsDto
            {
                Id = entity.Id,
                ReportNumber = entity.ReportNumber,
                InspectionDate = entity.InspectionDate,
                ExpiryDate = entity.ExpiryDate,
                Passed = entity.Passed,
                Notes = entity.Notes,
                VehicleId = entity.VehicleId,
                VehicleVIN = entity.Vehicle.VIN,
                VehicleBrand = entity.Vehicle.CarBrand,
                VehicleModel = entity.Vehicle.CarModel,
                AnnualReviewCompanyId = entity.AnnualReviewCompanyId,
                AnnualReviewCompanyName = entity.AnnualReviewCompany.Name
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

        public async Task<int> AddReportAsync(AnnualReportFormDto dto)
        {
            var createdById = dto.CreatedById;
            if (string.IsNullOrEmpty(createdById))
            {
                createdById = await this.dbContext.Users
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();
            }

            var entity = new AnnualReport
            {
                VehicleId = dto.VehicleId,
                AnnualReviewCompanyId = dto.AnnualReviewCompanyId,
                ReportNumber = dto.ReportNumber,
                InspectionDate = dto.InspectionDate,
                ExpiryDate = dto.ExpiryDate,
                Passed = dto.Passed,
                Notes = dto.Notes,
                CreatedOn = DateTime.UtcNow,
                CreatedById = createdById!
            };

            await this.dbContext.AnnualReports.AddAsync(entity);
            await this.dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> DeleteReportAsync(int id)
        {
            var entity = await this.dbContext.AnnualReports
                .FirstOrDefaultAsync(r => r.Id == id);

            if (entity == null)
            {
                return false;
            }

            this.dbContext.AnnualReports.Remove(entity);
            await this.dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignUserAsync(string userId, int? companyId)
        {
            var user = await this.dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            if (companyId.HasValue)
            {
                var exists = await this.dbContext.AnnualReviewCompanies
                    .AnyAsync(c => c.Id == companyId.Value);
                if (!exists)
                {
                    return false;
                }
            }

            user.AnnualReviewCompanyId = companyId;
            await this.dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int?> GetCompanyIdByReportIdAsync(int reportId)
        {
            return await this.dbContext.AnnualReports
                .AsNoTracking()
                .Where(r => r.Id == reportId)
                .Select(r => (int?)r.AnnualReviewCompanyId)
                .FirstOrDefaultAsync();
        }
    }
}
