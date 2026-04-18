using System.ComponentModel.DataAnnotations;

namespace VMAPP.Data.Models
{
    using VMAPP.Common;
    using VMAPP.Data.Models.Base;

    public class AnnualReport : IAuditInfo, IDeletableEntity
    {
        [Key]
        public int Id { get; set; }

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public int AnnualReviewCompanyId { get; set; }
        public AnnualReviewCompany AnnualReviewCompany { get; set; } = null!;

        [MaxLength(GlobalConstant.AnnualReportNumber)]
        public string? ReportNumber { get; set; }

        [Required]
        public DateTime InspectionDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public bool Passed { get; set; }

        [MaxLength(GlobalConstant.AnnualReportNotes)]
        public string? Notes { get; set; }

        public string CreatedById { get; set; } = null!;
        public ApplicationUser CreatedBy { get; set; } = null!;

        public string? ModifiedById { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
