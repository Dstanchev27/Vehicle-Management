namespace VMAPP.Services.DTOs.InsuranceDTOs
{
    public class InsurancePolicyDetailsDto
    {
        public int Id { get; set; }
        public string? PolicyNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int VehicleId { get; set; }
        public string VehicleVIN { get; set; } = string.Empty;
        public string VehicleBrand { get; set; } = string.Empty;
        public string VehicleModel { get; set; } = string.Empty;
        public int InsuranceCompanyId { get; set; }
        public string InsuranceCompanyName { get; set; } = string.Empty;
        public List<InsuranceClaimDto> Claims { get; set; } = new();
    }

    public class InsuranceClaimDto
    {
        public int Id { get; set; }
        public DateTime ClaimDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
