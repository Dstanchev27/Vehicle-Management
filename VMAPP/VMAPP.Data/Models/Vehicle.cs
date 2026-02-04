using System.ComponentModel.DataAnnotations;
using VMAPP.Common;
using VMAPP.Data.Models.Enums;

namespace VMAPP.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }

        [Required]
        public string VIN { get; set; }

        [MaxLength(GlobalConstant.CarBrandLength)]
        public  string CarBrand { get; set; } = null!;

        [MaxLength(GlobalConstant.CarModelLength)]
        public string CarModel { get; set; } = null!;

        public DateTime CreatedOnYear { get; set; }

        public string Color { get; set; } = null!;

        public  VehicleType VehicleType { get; set; }

        // Navigation property
        public ICollection<ServiceRecord> ServiceRecords { get; set; } = new HashSet<ServiceRecord>();
        public ICollection<VehicleVehicleService> VehicleVehicleServices { get; set; }
            = new HashSet<VehicleVehicleService>();
    }
}
