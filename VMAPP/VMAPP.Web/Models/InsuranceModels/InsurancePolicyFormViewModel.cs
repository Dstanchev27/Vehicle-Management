using System.ComponentModel.DataAnnotations;

namespace VMAPP.Web.Models.InsuranceModels
{
    public class InsurancePolicyFormViewModel
    {
        [Required]
        public int InsuranceCompanyId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [MaxLength(50)]
        public string? PolicyNumber { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }

    public class DeletePolicyRequest
    {
        public int Id { get; set; }
    }
}
