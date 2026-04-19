namespace VMAPP.Services.DTOs.AnnualReviewDTOs
{
    public class AnnualReportFormDto
    {
        public int VehicleId { get; set; }
        public int AnnualReviewCompanyId { get; set; }
        public string? ReportNumber { get; set; }
        public DateTime InspectionDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Passed { get; set; }
        public string? Notes { get; set; }
        public string? CreatedById { get; set; }
    }
}
