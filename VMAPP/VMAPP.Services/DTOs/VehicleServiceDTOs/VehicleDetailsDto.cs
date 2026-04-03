namespace VMAPP.Services.DTOs.VehicleServiceDTOs
{
    using VMAPP.Services.DTOs.VehicleDTOs;

    public class VehicleDetailsDto
    {
        public VehicleDto Vehicle { get; set; } = new();
        public List<ServiceRecordDto> ServiceRecords { get; set; } = new();
    }
}
