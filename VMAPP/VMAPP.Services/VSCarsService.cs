using Microsoft.EntityFrameworkCore;
using VMAPP.Data;
using VMAPP.Data.Models;
using VMAPP.Data.Models.Enums;
using VMAPP.Services.DTOs;
using VMAPP.Services.Interfaces;

namespace VMAPP.Services
{
    public class VSCarsService : IVSCarsService
    {
        private readonly ApplicationDbContext dbContext;

        public VSCarsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ServiceWithVehiclesDto?> GetServiceWithVehiclesByNameAsync(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                return null;
            }

            return await dbContext.VehicleServices
                .AsNoTracking()
                .Where(s => s.Name == serviceName)
                .Select(s => new ServiceWithVehiclesDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Vehicles = s.ServiceRecords
                        .Select(sr => sr.Vehicle)
                        .Distinct()
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
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> AddVehicleToServiceAsync(int serviceId, VehicleDto vehicle)
        {
            var dbVehicle = new Vehicle
            {
                VIN = vehicle.VIN,
                CarBrand = vehicle.CarBrand,
                CarModel = vehicle.CarModel,
                CreatedOnYear = vehicle.CreatedOnYear,
                Color = vehicle.Color,
                VehicleType = (VehicleType)vehicle.VehicleType
            };

            await dbContext.Vehicles.AddAsync(dbVehicle);
            await dbContext.SaveChangesAsync();

            return dbVehicle.VehicleId;
        }

        public async Task<VehicleDetailsDto?> GetVehicleDetailsAsync(int vehicleId)
        {
            var vehicleEntity = await dbContext.Vehicles
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);

            if (vehicleEntity == null)
            {
                return null;
            }

            var vehicleDto = new VehicleDto
            {
                Id = vehicleEntity.VehicleId,
                VIN = vehicleEntity.VIN,
                CarBrand = vehicleEntity.CarBrand,
                CarModel = vehicleEntity.CarModel,
                CreatedOnYear = vehicleEntity.CreatedOnYear,
                Color = vehicleEntity.Color,
                VehicleType = vehicleEntity.VehicleType
            };

            var records = await dbContext.ServiceRecords
                .AsNoTracking()
                .Where(r => r.VehicleId == vehicleId)
                .OrderByDescending(r => r.ServiceDate)
                .Select(r => new ServiceRecordDto
                {
                    Id = r.ServiceRecordId,
                    VehicleId = r.VehicleId,
                    VehicleServiceId = r.VehicleServiceId,
                    ServiceDate = r.ServiceDate,
                    Cost = r.Cost,
                    Description = r.Description
                })
                .ToListAsync();

            return new VehicleDetailsDto
            {
                Vehicle = vehicleDto,
                ServiceRecords = records
            };
        }

        public async Task<bool> UpdateVehicleAsync(VehicleDto vehicle)
        {
            var dbVehicle = await dbContext.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == vehicle.Id);

            if (dbVehicle == null)
            {
                return false;
            }

            dbVehicle.VIN = vehicle.VIN;
            dbVehicle.CarBrand = vehicle.CarBrand;
            dbVehicle.CarModel = vehicle.CarModel;
            dbVehicle.CreatedOnYear = vehicle.CreatedOnYear;
            dbVehicle.Color = vehicle.Color;
            dbVehicle.VehicleType = (VehicleType)vehicle.VehicleType;

            dbContext.Vehicles.Update(dbVehicle);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<VehicleWithServiceRecordsDto?> GetVehicleWithServiceRecordsAsync(int vehicleId, int serviceId)
        {
            return await dbContext.Vehicles
                .AsNoTracking()
                .Where(v => v.VehicleId == vehicleId)
                .Select(v => new VehicleWithServiceRecordsDto
                {
                    Id = v.VehicleId,
                    VIN = v.VIN,
                    CarBrand = v.CarBrand,
                    CarModel = v.CarModel,
                    CreatedOnYear = v.CreatedOnYear,
                    Color = v.Color,
                    VehicleType = v.VehicleType.ToString(),
                    ServiceRecords = v.ServiceRecords
                        .Where(sr => sr.VehicleServiceId == serviceId)
                        .OrderByDescending(sr => sr.ServiceDate)
                        .Select(sr => new ServiceRecordDto
                        {
                            Id = sr.ServiceRecordId,
                            Cost = sr.Cost,
                            Description = sr.Description,
                            ServiceDate = sr.ServiceDate,
                            VehicleId = sr.VehicleId,
                            VehicleServiceId = sr.VehicleServiceId
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceRecordDto?> GetServiceRecordByIdAsync(int id)
        {
            var sr = await dbContext.ServiceRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ServiceRecordId == id);

            if (sr == null)
            {
                return null;
            }

            return new ServiceRecordDto
            {
                Id = sr.ServiceRecordId,
                Cost = sr.Cost,
                Description = sr.Description,
                ServiceDate = sr.ServiceDate,
                VehicleId = sr.VehicleId,
                VehicleServiceId = sr.VehicleServiceId
            };
        }

        public async Task<int> AddServiceRecordAsync(ServiceRecordDto dto)
        {
            var record = new ServiceRecord
            {
                Cost = dto.Cost,
                Description = dto.Description,
                ServiceDate = dto.ServiceDate,
                VehicleId = dto.VehicleId,
                VehicleServiceId = dto.VehicleServiceId
            };

            await dbContext.ServiceRecords.AddAsync(record);
            await dbContext.SaveChangesAsync();
            return record.ServiceRecordId;
        }

        public async Task<bool> UpdateServiceRecordAsync(ServiceRecordDto dto)
        {
            var record = await dbContext.ServiceRecords
                .FirstOrDefaultAsync(s => s.ServiceRecordId == dto.Id);

            if (record == null)
            {
                return false;
            }

            record.Cost = dto.Cost;
            record.Description = dto.Description;
            record.ServiceDate = dto.ServiceDate;

            dbContext.ServiceRecords.Update(record);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteServiceRecordAsync(int id)
        {
            var record = await dbContext.ServiceRecords
                .FirstOrDefaultAsync(s => s.ServiceRecordId == id);

            if (record == null)
            {
                return false;
            }

            dbContext.ServiceRecords.Remove(record);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVehicleAsync(int vehicleId)
        {
            var vehicle = await dbContext.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
            if (vehicle == null)
            {
                return false;
            }

            dbContext.Vehicles.Remove(vehicle);
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
