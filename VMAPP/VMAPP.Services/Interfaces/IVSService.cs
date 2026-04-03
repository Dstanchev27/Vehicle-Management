using VMAPP.Services.DTOs.VehicleDTOs;

namespace VMAPP.Services.Interfaces
{
    public interface IVSService
    {
        Task<IReadOnlyList<VehicleDto>> GetAllAsync();
        Task<VehicleDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(VehicleDto dto);
        Task<bool> UpdateAsync(VehicleDto dto);
        Task<bool> DeleteAsync(int id);
    }
}