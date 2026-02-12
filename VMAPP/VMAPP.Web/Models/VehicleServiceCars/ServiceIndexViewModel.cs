namespace VMAPP.Web.Models.VehicleServiceCars
{
    public class ServiceIndexViewModel
    {
        public int? Id { get; set; }

        public string Name { get; set; } 

        public ICollection<VehicleServiceCarModel> Cars { get; set; } = new List<VehicleServiceCarModel>();
    }
}
