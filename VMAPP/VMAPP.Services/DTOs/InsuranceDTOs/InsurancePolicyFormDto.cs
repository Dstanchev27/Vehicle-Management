namespace VMAPP.Services.DTOs.InsuranceDTOs
{
    public class InsurancePolicyFormDto
    {
        public int VehicleId { get; set; }
        public int InsuranceCompanyId { get; set; }
        public string? PolicyNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? CreatedById { get; set; }
    }
}
