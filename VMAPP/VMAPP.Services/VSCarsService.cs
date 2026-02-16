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
        private readonly ApplicationDbContext _dbContext;

        public VSCarsService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServiceWithVehiclesDto?> GetServiceWithVehiclesByNameAsync(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                return null;
            }

            return await _dbContext.VehicleServices
                .AsNoTracking()
                .Where(s => s.Name == serviceName)
                .Select(s => new ServiceWithVehiclesDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Vehicles = s.VehicleVehicleServices
                        .Select(vs => new VehicleDto
                        {
                            Id = vs.Vehicle.VehicleId,
                            VIN = vs.Vehicle.VIN,
                            CarBrand = vs.Vehicle.CarBrand,
                            CarModel = vs.Vehicle.CarModel,
                            CreatedOnYear = vs.Vehicle.CreatedOnYear,
                            Color = vs.Vehicle.Color,
                            VehicleType = (int)vs.Vehicle.VehicleType
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

            var vehicleServiceVehicle = new VehicleVehicleService
            {
                VehicleServiceId = serviceId,
                Vehicle = dbVehicle
            };

            dbVehicle.VehicleVehicleServices.Add(vehicleServiceVehicle);

            await _dbContext.Vehicles.AddAsync(dbVehicle);
            await _dbContext.SaveChangesAsync();

            return dbVehicle.VehicleId;
        }

        public async Task<VehicleDetailsDto?> GetVehicleDetailsAsync(int vehicleId)
        {
            var vehicleEntity = await _dbContext.Vehicles
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
                VehicleType = (int)vehicleEntity.VehicleType
            };

            var records = await _dbContext.ServiceRecords
                .AsNoTracking()
                .Where(r => r.VehicleId == vehicleId)
                .OrderByDescending(r => r.ServiceDate)
                .Select(r => new ServiceRecordDto
                {
                    Id = r.ServiceRecordId,
                    VehicleId = r.VehicleId,
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
            var dbVehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == vehicle.Id);
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

            _dbContext.Vehicles.Update(dbVehicle);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<int> AddServiceRecordAsync(ServiceRecordDto record)
        {
            var dbRecord = new ServiceRecord
            {
                ServiceDate = record.ServiceDate,
                Cost = record.Cost,
                Description = record.Description ?? string.Empty,
                VehicleId = record.VehicleId
            };

            await _dbContext.ServiceRecords.AddAsync(dbRecord);
            await _dbContext.SaveChangesAsync();

            return dbRecord.ServiceRecordId;
        }

        public async Task<bool> UpdateServiceRecordAsync(ServiceRecordDto record)
        {
            var dbRecord = await _dbContext.ServiceRecords.FirstOrDefaultAsync(r => r.ServiceRecordId == record.Id);
            if (dbRecord == null)
            {
                return false;
            }

            dbRecord.ServiceDate = record.ServiceDate;
            dbRecord.Cost = record.Cost;
            dbRecord.Description = record.Description ?? string.Empty;

            _dbContext.ServiceRecords.Update(dbRecord);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteServiceRecordAsync(int recordId)
        {
            var dbRecord = await _dbContext.ServiceRecords.FirstOrDefaultAsync(r => r.ServiceRecordId == recordId);
            if (dbRecord == null)
            {
                return false;
            }

            _dbContext.ServiceRecords.Remove(dbRecord);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteVehicleAsync(int vehicleId)
        {
            var vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
            if (vehicle == null)
            {
                return false;
            }

            _dbContext.Vehicles.Remove(vehicle);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
