namespace VMAPP.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class VehicleVehicleService
    {
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;
        public int VehicleServiceId { get; set; }
        public VehicleService VehicleService { get; set; } = null!;
    }
}
