namespace VMAPP.Services.DTOs.VehicleServiceDTOs
{
    public class ServiceRecordDto
    {
        public int Id { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime ServiceDate { get; set; }
        public int VehicleId { get; set; }
        public int VehicleServiceId { get; set; }
        public string? CreatedById { get; set; }
    }
}
