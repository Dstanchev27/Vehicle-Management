using VMAPP.Services.DTOs.InsuranceDTOs;
using VMAPP.Services.DTOs.VehicleDTOs;

namespace VMAPP.Services.Interfaces
{
    public interface IVSInsuranceService
    {
        Task<IReadOnlyList<InsuranceCompanyDto>> GetAllAsync();
        Task<InsuranceCompanyDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(InsuranceCompanyDto dto);
        Task<bool> UpdateAsync(InsuranceCompanyDto dto);
        Task<bool> DeleteAsync(int id);
        Task<InsuranceCompanyWithVehiclesDto?> GetCompanyWithVehiclesAsync(int id);
        Task<InsurancePolicyDetailsDto?> GetPolicyDetailsAsync(int id);
        Task<VehicleDto?> GetVehicleByVinAsync(string vin);
        Task<int> AddPolicyAsync(InsurancePolicyFormDto dto);
        Task<bool> DeletePolicyAsync(int id);
        Task<InsuranceClaimFormDto?> GetClaimByIdAsync(int id);
        Task<int> AddClaimAsync(InsuranceClaimFormDto dto);
        Task<bool> DeleteClaimAsync(int id);
    }
}
