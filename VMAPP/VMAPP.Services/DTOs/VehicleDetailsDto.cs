namespace VMAPP.Services.DTOs
{
    public class VehicleDetailsDto
    {
        public VehicleDto Vehicle { get; set; } = new();
        public List<ServiceRecordDto> ServiceRecords { get; set; } = new();
    }
}
