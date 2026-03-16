using VMAPP.Data.Models.Enums;

namespace VMAPP.Web.Models.VehicleViewModels
{
    public class VehicleIndexViewModel
    {
        public int Id { get; set; }
        public string VIN { get; set; } = string.Empty;
        public string CarBrand { get; set; } = string.Empty;
        public string CarModel { get; set; } = string.Empty;
        public int CreatedOnYear { get; set; }
        public string Color { get; set; } = string.Empty;
        public VehicleType VehicleType { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}