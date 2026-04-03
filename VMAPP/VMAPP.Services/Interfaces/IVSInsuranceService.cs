using VMAPP.Services.DTOs;

namespace VMAPP.Services.Interfaces
{
    public interface IVSInsuranceService
    {
        Task<IReadOnlyList<InsuranceCompanyDto>> GetAllAsync();
        Task<InsuranceCompanyDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(InsuranceCompanyDto dto);
        Task<bool> UpdateAsync(InsuranceCompanyDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
