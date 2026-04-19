using VMAPP.Data.Models.Enums;

namespace VMAPP.Services.DTOs.AnnualReviewDTOs
{
    public class AnnualReviewCompanyWithVehiclesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public List<VehicleWithReportIdDto> Vehicles { get; set; } = new();
    }

    public class VehicleWithReportIdDto
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
