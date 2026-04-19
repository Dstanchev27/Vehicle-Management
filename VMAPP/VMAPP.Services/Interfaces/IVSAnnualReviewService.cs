using VMAPP.Services.DTOs.AnnualReviewDTOs;
using VMAPP.Services.DTOs.VehicleDTOs;

namespace VMAPP.Services.Interfaces
{
    public interface IVSAnnualReviewService
    {
        Task<IReadOnlyList<AnnualReviewCompanyDto>> GetAllAsync();
        Task<AnnualReviewCompanyDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(AnnualReviewCompanyDto dto);
        Task<bool> UpdateAsync(AnnualReviewCompanyDto dto);
        Task<bool> DeleteAsync(int id);
        Task<AnnualReviewCompanyWithVehiclesDto?> GetCompanyWithVehiclesAsync(int id);
        Task<AnnualReportDetailsDto?> GetReportDetailsAsync(int id);
        Task<VehicleDto?> GetVehicleByVinAsync(string vin);
        Task<int> AddReportAsync(AnnualReportFormDto dto);
        Task<bool> DeleteReportAsync(int id);
        Task<bool> AssignUserAsync(string userId, int? companyId);
        Task<int?> GetCompanyIdByReportIdAsync(int reportId);
    }
}
