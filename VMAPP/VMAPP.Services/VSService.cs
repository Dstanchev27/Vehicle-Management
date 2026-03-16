using Microsoft.EntityFrameworkCore;
using VMAPP.Data;
using VMAPP.Data.Models;
using VMAPP.Services.DTOs;
using VMAPP.Services.Interfaces;

namespace VMAPP.Services
{
    public class VSService : IVSService
    {
        private readonly ApplicationDbContext dbContext;

        public VSService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IReadOnlyList<VehicleDto>> GetAllAsync()
        {
            return await this.dbContext.Vehicles
                .AsNoTracking()
                .OrderBy(v => v.CarBrand)
                .ThenBy(v => v.CarModel)
                .Select(v => new VehicleDto
                {
                    Id = v.VehicleId,
                    VIN = v.VIN,
                    CarBrand = v.CarBrand,
                    CarModel = v.CarModel,
                    CreatedOnYear = v.CreatedOnYear,
                    Color = v.Color,
                    VehicleType = v.VehicleType,
                    CreatedOn = v.CreatedOn
                })
                .ToListAsync();
        }

        public async Task<VehicleDto?> GetByIdAsync(int id)
        {
            var entity = await this.dbContext.Vehicles
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.VehicleId == id);

            if (entity == null)
            {
                return null;
            }

            return new VehicleDto
            {
                Id = entity.VehicleId,
                VIN = entity.VIN,
                CarBrand = entity.CarBrand,
                CarModel = entity.CarModel,
                CreatedOnYear = entity.CreatedOnYear,
                Color = entity.Color,
                VehicleType = entity.VehicleType,
                CreatedOn = entity.CreatedOn
            };
        }

        public async Task<int> CreateAsync(VehicleDto dto)
        {
            var entity = new Vehicle
            {
                VIN = dto.VIN,
                CarBrand = dto.CarBrand,
                CarModel = dto.CarModel,
                CreatedOnYear = dto.CreatedOnYear,
                Color = dto.Color,
                VehicleType = dto.VehicleType,
                CreatedOn = DateTime.UtcNow
            };

            await this.dbContext.Vehicles.AddAsync(entity);
            await this.dbContext.SaveChangesAsync();

            return entity.VehicleId;
        }

        public async Task<bool> UpdateAsync(VehicleDto dto)
        {
            var entity = await this.dbContext.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == dto.Id);

            if (entity == null)
            {
                return false;
            }

            entity.VIN = dto.VIN;
            entity.CarBrand = dto.CarBrand;
            entity.CarModel = dto.CarModel;
            entity.CreatedOnYear = dto.CreatedOnYear;
            entity.Color = dto.Color;
            entity.VehicleType = dto.VehicleType;

            this.dbContext.Vehicles.Update(entity);
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await this.dbContext.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == id);

            if (entity == null)
            {
                return false;
            }

            this.dbContext.Vehicles.Remove(entity);
            await this.dbContext.SaveChangesAsync();

            return true;
        }
    }
}