using System.ComponentModel.DataAnnotations;

namespace VMAPP.Data.Models
{
    using VMAPP.Common;
    using VMAPP.Data.Models.Base;

    public class InsurancePolicy : IAuditInfo, IDeletableEntity
    {
        [Key]
        public int Id { get; set; }

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public string CreatedById { get; set; } = null!;
        public ApplicationUser CreatedBy { get; set; } = null!;

        public string? ModifiedById { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }

        public int InsuranceCompanyId { get; set; }
        public InsuranceCompany InsuranceCompany { get; set; } = null!;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [MaxLength(GlobalConstant.InsurancePolicyNumber)]
        public string? PolicyNumber { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }

        public ICollection<InsuranceClaim> Claims { get; set; } = new HashSet<InsuranceClaim>();
    }
}
