using Microsoft.EntityFrameworkCore;
using VMAPP.Data;
using VMAPP.Data.Models;
using VMAPP.Services.DTOs;
using VMAPP.Services.Interfaces;

namespace VMAPP.Services
{
    public class VSManagementService : IVSManagementService
    {
        private readonly ApplicationDbContext dbContext;

        public VSManagementService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IReadOnlyList<VehicleServiceDto>> GetAllAsync()
        {
            return await dbContext.VehicleServices
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
            var entity = await dbContext.VehicleServices
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

            await dbContext.VehicleServices.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(VehicleServiceDto dto)
        {
            var entity = await dbContext.VehicleServices.FirstOrDefaultAsync(s => s.Id == dto.Id);

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

            dbContext.VehicleServices.Update(entity);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await dbContext.VehicleServices.FirstOrDefaultAsync(s => s.Id == id);

            if (entity == null)
            {
                return false;
            }

            dbContext.VehicleServices.Remove(entity);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await dbContext.VehicleServices.AnyAsync(s => s.Id == id);
        }

        public async Task<IReadOnlyList<VehicleDto>> GetVehiclesByServiceIdAsync(int serviceId)
        {
            return await dbContext.ServiceRecords
                .AsNoTracking()
                .Where(sr => sr.VehicleServiceId == serviceId)
                .Select(sr => new VehicleDto
                {
                    Id = sr.Vehicle.VehicleId,
                    VIN = sr.Vehicle.VIN,
                    CarBrand = sr.Vehicle.CarBrand,
                    CarModel = sr.Vehicle.CarModel,
                    CreatedOnYear = sr.Vehicle.CreatedOnYear,
                    Color = sr.Vehicle.Color,
                    VehicleType = sr.Vehicle.VehicleType
                })
                .ToListAsync();
        }

        public async Task<ServiceWithVehiclesDto?> GetVehiclesServiceDetailsByIdAsync(int id)
        {
            var service = await dbContext.VehicleServices
                .AsNoTracking()
                .Where(vs => vs.Id == id)
                .Select(vs => new ServiceWithVehiclesDto
                {
                    Id = vs.Id,
                    Name = vs.Name,
                    City = vs.City,
                    Address = vs.Address,
                    Email = vs.Email,
                    Phone = vs.Phone,
                    Description = vs.Description,
                    CreatedOn = vs.CreatedOn
                })
                .FirstOrDefaultAsync();

            if (service == null)
            {
                return null;
            }

            var vehicleIds = await dbContext.ServiceRecords
                .AsNoTracking()
                .Where(sr => sr.VehicleServiceId == id)
                .Select(sr => sr.VehicleId)
                .Distinct()
                .ToListAsync();

            service.Vehicles = await dbContext.Vehicles
                .AsNoTracking()
                .Where(v => vehicleIds.Contains(v.VehicleId))
                .Select(v => new VehicleDto
                {
                    Id = v.VehicleId,
                    VIN = v.VIN,
                    CarBrand = v.CarBrand,
                    CarModel = v.CarModel,
                    CreatedOnYear = v.CreatedOnYear,
                    Color = v.Color,
                    VehicleType = v.VehicleType
                })
                .ToListAsync();

            return service;
        }

        public async Task<VehicleDto?> GetVehicleByVinAsync(string vin)
        {
            var vehicle = await dbContext.Vehicles
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

        public async Task<bool> AddVehicleToServiceAsync(int serviceId, int vehicleId)
        {
            var alreadyAssigned = await dbContext.ServiceRecords
                .AnyAsync(sr => sr.VehicleServiceId == serviceId && sr.VehicleId == vehicleId);

            if (alreadyAssigned)
            {
                return false;
            }

            var record = new ServiceRecord
            {
                VehicleServiceId = serviceId,
                VehicleId = vehicleId,
                Cost = 0,
                Description = $"Vehicle accepted into service on {DateTime.UtcNow:dd MMMM yyyy}.",
                ServiceDate = DateTime.UtcNow
            };

            await dbContext.ServiceRecords.AddAsync(record);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<(bool Success, string? Message)> RemoveVehicleFromServiceAsync(int serviceId, int vehicleId)
        {
            var records = await dbContext.ServiceRecords
                .Where(sr => sr.VehicleServiceId == serviceId && sr.VehicleId == vehicleId)
                .ToListAsync();

            if (!records.Any())
            {
                return (false, "Vehicle is not assigned to this service.");
            }

            if (records.Any(r => r.Cost > 0))
            {
                return (false, "This vehicle cannot be removed because it has service records with costs. Remove all paid service records first.");
            }

            dbContext.ServiceRecords.RemoveRange(records);
            await dbContext.SaveChangesAsync();

            return (true, null);
        }
    }
}