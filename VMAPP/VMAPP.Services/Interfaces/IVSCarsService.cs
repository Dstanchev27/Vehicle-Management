using VMAPP.Services.DTOs.VehicleDTOs;
using VMAPP.Services.DTOs.VehicleServiceDTOs;

namespace VMAPP.Services.Interfaces
{
    public interface IVSCarsService
    {
        Task<ServiceWithVehiclesDto?> GetServiceWithVehiclesByNameAsync(string serviceName);
        Task<int> AddVehicleToServiceAsync(int serviceId, VehicleDto vehicle);
        Task<VehicleDetailsDto?> GetVehicleDetailsAsync(int vehicleId);
        Task<bool> UpdateVehicleAsync(VehicleDto vehicle);
        Task<bool> DeleteVehicleAsync(int vehicleId);
        Task<VehicleWithServiceRecordsDto?> GetVehicleWithServiceRecordsAsync(int vehicleId, int serviceId);
        Task<ServiceRecordDto?> GetServiceRecordByIdAsync(int id);
        Task<int> AddServiceRecordAsync(ServiceRecordDto dto);
        Task<bool> UpdateServiceRecordAsync(ServiceRecordDto dto);
        Task<bool> DeleteServiceRecordAsync(int id);
    }
}
