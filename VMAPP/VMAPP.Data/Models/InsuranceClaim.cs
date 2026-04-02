using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VMAPP.Data.Models
{
    using VMAPP.Common;
    using VMAPP.Data.Models.Base;

    public class InsuranceClaim : IAuditInfo, IDeletableEntity
    {
        [Key]
        public int Id { get; set; }

        public int InsurancePolicyId { get; set; }
        public InsurancePolicy InsurancePolicy { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime ClaimDate { get; set; }

        [Required]
        [MaxLength(GlobalConstant.InsuranceClaimDescription)]
        public string Description { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
