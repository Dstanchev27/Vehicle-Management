namespace VMAPP.Web.Models.VehicleServiceCars
{
    public class ServiceVehicleViewModel
    {
        public int VehicleId { get; set; }
        public int ServiceId { get; set; }
        public string VIN { get; set; } = string.Empty;
        public string CarBrand { get; set; } = string.Empty;
        public string CarModel { get; set; } = string.Empty;
        public int CreatedOnYear { get; set; }
        public string Color { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public List<ServiceRecordRowViewModel> ServiceRecords { get; set; } = new();
    }

    public class ServiceRecordRowViewModel
    {
        public int Id { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime ServiceDate { get; set; }
    }
}