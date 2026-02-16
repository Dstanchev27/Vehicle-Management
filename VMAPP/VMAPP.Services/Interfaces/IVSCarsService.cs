using VMAPP.Services.DTOs;

namespace VMAPP.Services.Interfaces
{
    public interface IVSCarsService
    {
        Task<ServiceWithVehiclesDto?> GetServiceWithVehiclesByNameAsync(string serviceName);
        Task<int> AddVehicleToServiceAsync(int serviceId, VehicleDto vehicle);
        Task<VehicleDetailsDto?> GetVehicleDetailsAsync(int vehicleId);
        Task<bool> UpdateVehicleAsync(VehicleDto vehicle);
        Task<int> AddServiceRecordAsync(ServiceRecordDto record);
        Task<bool> UpdateServiceRecordAsync(ServiceRecordDto record);
        Task<bool> DeleteServiceRecordAsync(int recordId);
        Task<bool> DeleteVehicleAsync(int vehicleId);
    }
}
