namespace VMAPP.Services.DTOs.InsuranceDTOs
{
    public class InsuranceClaimFormDto
    {
        public int Id { get; set; }
        public int InsurancePolicyId { get; set; }
        public DateTime ClaimDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
