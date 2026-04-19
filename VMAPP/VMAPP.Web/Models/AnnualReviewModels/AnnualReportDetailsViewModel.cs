namespace VMAPP.Web.Models.AnnualReviewModels
{
    public class AnnualReportDetailsViewModel
    {
        public int Id { get; set; }
        public string? ReportNumber { get; set; }
        public DateTime InspectionDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Passed { get; set; }
        public string? Notes { get; set; }
        public int VehicleId { get; set; }
        public string VehicleVIN { get; set; } = string.Empty;
        public string VehicleBrand { get; set; } = string.Empty;
        public string VehicleModel { get; set; } = string.Empty;
        public int AnnualReviewCompanyId { get; set; }
        public string AnnualReviewCompanyName { get; set; } = string.Empty;
    }
}
