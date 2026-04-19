using System.ComponentModel.DataAnnotations;

namespace VMAPP.Web.Models.AnnualReviewModels
{
    public class AnnualReportFormViewModel
    {
        [Required]
        public int AnnualReviewCompanyId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [MaxLength(50)]
        public string? ReportNumber { get; set; }

        [Required]
        public DateTime InspectionDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public bool Passed { get; set; }

        [StringLength(1500)]
        public string? Notes { get; set; }
    }

    public class DeleteReportRequest
    {
        public int Id { get; set; }
    }
}
