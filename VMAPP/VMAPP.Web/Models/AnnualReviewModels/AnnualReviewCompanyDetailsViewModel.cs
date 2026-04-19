using VMAPP.Data.Models.Enums;

namespace VMAPP.Web.Models.AnnualReviewModels
{
    public class AnnualReviewCompanyDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public List<VehicleWithReportRowViewModel> Vehicles { get; set; } = new();
    }

    public class VehicleWithReportRowViewModel
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string VIN { get; set; } = string.Empty;
        public string CarBrand { get; set; } = string.Empty;
        public string CarModel { get; set; } = string.Empty;
        public int CreatedOnYear { get; set; }
        public string Color { get; set; } = string.Empty;
        public VehicleType VehicleType { get; set; }
        public string? ReportNumber { get; set; }
    }
}
