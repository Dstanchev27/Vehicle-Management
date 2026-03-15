using VMAPP.Services.DTOs;

namespace VMAPP.Services.Interfaces
{
    public interface IVSManagementService
    {
        Task<IReadOnlyList<VehicleServiceDto>> GetAllAsync();
        Task<VehicleServiceDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(VehicleServiceDto dto);
        Task<bool> UpdateAsync(VehicleServiceDto dto);
        Task<ServiceWithVehiclesDto?> GetVehiclesServiceDetailsByIdAsync(int id);
        Task<IReadOnlyList<VehicleDto>> GetVehiclesByServiceIdAsync(int serviceId);
        Task<bool> DeleteAsync(int id);
    }
}
