namespace VMAPP.Web.Models.VehicleServiceCars
{
    using VMAPP.Data.Models;
    using VMAPP.Data.Models.Enums;

    public class AddVehicleViewModel
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int VehicleId { get; set; }
        public string VIN { get; set; }

        public string CarBrand { get; set; } = null!;
        public string CarModel { get; set; } = null!;
        public int CreatedOnYear { get; set; }
        public string Color { get; set; } = null!;
        public VehicleType VehicleType { get; set; }
        public ICollection<ServiceRecordViewModel> ServiceRecord { get; set; } = new List<ServiceRecordViewModel>();
    }
}
