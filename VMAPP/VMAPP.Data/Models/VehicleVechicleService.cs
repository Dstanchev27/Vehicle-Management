namespace VMAPP.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class VehicleVechicleService
    {
        public int VechicleId { get; set; }
        public Vehicle Vechicle { get; set; } = null!;
        public int VechicleServiceId { get; set; }
        public VehicleService VehicleService { get; set; } = null!;
    }
}
