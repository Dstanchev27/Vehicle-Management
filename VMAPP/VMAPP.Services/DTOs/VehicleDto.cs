namespace VMAPP.Services.DTOs
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public string VIN { get; set; } = string.Empty;
        public string CarBrand { get; set; } = string.Empty;
        public string CarModel { get; set; } = string.Empty;
        public int CreatedOnYear { get; set; }
        public string Color { get; set; } = string.Empty;
        public int VehicleType { get; set; }
    }
}
