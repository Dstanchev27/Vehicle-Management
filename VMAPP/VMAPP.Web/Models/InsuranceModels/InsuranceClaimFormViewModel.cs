using System.ComponentModel.DataAnnotations;

namespace VMAPP.Web.Models.InsuranceModels
{
    public class InsuranceClaimFormViewModel
    {
        [Required]
        public int InsurancePolicyId { get; set; }

        [Required]
        public DateTime ClaimDate { get; set; }

        [Required]
        [StringLength(1500, MinimumLength = 3,
            ErrorMessage = "Description must be between 3 and 1500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, 999999999.99, ErrorMessage = "Amount must be 0 or greater.")]
        public decimal Amount { get; set; }
    }

    public class DeleteClaimRequest
    {
        public int Id { get; set; }
    }
}
