using Microsoft.EntityFrameworkCore;
using VMAPP.Data;
using VMAPP.Data.Models;
using VMAPP.Services.DTOs;
using VMAPP.Services.Interfaces;

namespace VMAPP.Services
{
    public class VSManagementService : IVSManagementService
    {
        private readonly ApplicationDbContext _dbContext;

        public VSManagementService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<VehicleServiceDto>> GetAllAsync()
        {
            return await _dbContext.VehicleServices
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ThenBy(s => s.CreatedOn)
                .Select(s => new VehicleServiceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    CreatedOn = s.CreatedOn,
                    City = s.City,
                    Address = s.Address,
                    Email = s.Email,
                    Phone = s.Phone
                })
                .ToListAsync();
        }

        public async Task<VehicleServiceDto?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.VehicleServices
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (entity == null)
            {
                return null;
            }

            return new VehicleServiceDto
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

        public async Task<int> CreateAsync(VehicleServiceDto dto)
        {
            var entity = new VehicleService
            {
                Name = dto.Name,
                Description = dto.Description,
                City = dto.City,
                Address = dto.Address,
                Email = dto.Email,
                Phone = dto.Phone,
                CreatedOn = dto.CreatedOn == default ? DateTime.UtcNow : dto.CreatedOn,
            };

            await _dbContext.VehicleServices.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(VehicleServiceDto dto)
        {
            var entity = await _dbContext.VehicleServices.FirstOrDefaultAsync(s => s.Id == dto.Id);
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

            _dbContext.VehicleServices.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbContext.VehicleServices.FirstOrDefaultAsync(s => s.Id == id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.VehicleServices.Remove(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
