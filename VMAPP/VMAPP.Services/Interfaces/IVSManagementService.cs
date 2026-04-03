using VMAPP.Services.DTOs.VehicleDTOs;
using VMAPP.Services.DTOs.VehicleServiceDTOs;

namespace VMAPP.Services.Interfaces
{
    public interface IVSManagementService
    {
        Task<IReadOnlyList<VehicleServiceDto>> GetAllAsync();
        Task<VehicleServiceDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(VehicleServiceDto dto);
        Task<bool> UpdateAsync(VehicleServiceDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IReadOnlyList<VehicleDto>> GetVehiclesByServiceIdAsync(int serviceId);
        Task<ServiceWithVehiclesDto?> GetVehiclesServiceDetailsByIdAsync(int id);
        Task<VehicleDto?> GetVehicleByVinAsync(string vin);
        Task<bool> AddVehicleToServiceAsync(int serviceId, int vehicleId);
        Task<(bool Success, string? Message)> RemoveVehicleFromServiceAsync(int serviceId, int vehicleId);
    }
}
