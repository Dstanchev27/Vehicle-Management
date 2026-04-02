using System.ComponentModel.DataAnnotations;

namespace VMAPP.Data.Models
{
    using VMAPP.Common;
    using VMAPP.Data.Models.Base;

    public class InsuranceCompany : IAuditInfo, IDeletableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(GlobalConstant.InsuranceCompanyName)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(GlobalConstant.InsuranceCompanyDescription)]
        public string Description { get; set; } = null!;

        [Required]
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Address { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; } = null!;

        public string CreatedById { get; set; } = null!;
        public ApplicationUser CreatedBy { get; set; } = null!;

        public string? ModifiedById { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }

        public ICollection<InsurancePolicy> InsurancePolicies { get; set; }
            = new HashSet<InsurancePolicy>();
    }
}
