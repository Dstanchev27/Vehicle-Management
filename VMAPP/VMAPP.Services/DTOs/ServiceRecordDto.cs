namespace VMAPP.Services.DTOs
{
    public class ServiceRecordDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public DateTime ServiceDate { get; set; }
        public decimal Cost { get; set; }
        public string? Description { get; set; }
    }
}
