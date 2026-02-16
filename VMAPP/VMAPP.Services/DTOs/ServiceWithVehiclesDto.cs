namespace VMAPP.Services.DTOs
{
    public class ServiceWithVehiclesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<VehicleDto> Vehicles { get; set; } = new();
    }
}
