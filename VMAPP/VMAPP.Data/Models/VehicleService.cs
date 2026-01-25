using System.ComponentModel.DataAnnotations;

namespace VMAPP.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using VMAPP.Common;

    public class VehicleService
    {
        [Key]
        public required int Id { get; set; }
        [MaxLength(GlobalConstant.VechicleServiceName)]
        public required string Name { get; set; }
        //what the service is about
        [MaxLength(GlobalConstant.VechicleServiceDescription)]
        public string Description { get; set; }

        // Navigation property
        public ICollection<ServiceRecord> ServiceRecord { get; set; }
            = new HashSet<ServiceRecord>();
        public ICollection<VehicleVechicleService> VehicleVechicleServices { get; set; }
            = new HashSet<VehicleVechicleService>();
    }
}
