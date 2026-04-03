namespace VMAPP.Services.DTOs.VehicleServiceDTOs
{
    public class VehicleWithServiceRecordsDto
    {
        public int Id { get; set; }
        public string VIN { get; set; } = string.Empty;
        public string CarBrand { get; set; } = string.Empty;
        public string CarModel { get; set; } = string.Empty;
        public int CreatedOnYear { get; set; }
        public string Color { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public List<ServiceRecordDto> ServiceRecords { get; set; } = new();
    }
}