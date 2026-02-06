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
        public int Id { get; set; }
        [MaxLength(GlobalConstant.VehicleServiceName)]
        public string Name { get; set; }

        //what the service is about
        [MaxLength(GlobalConstant.VehicleServiceDescription)]
        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }
        public string City { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
        // Navigation property
        public ICollection<ServiceRecord> ServiceRecord { get; set; }
            = new HashSet<ServiceRecord>();
        public ICollection<VehicleVehicleService> VehicleVehicleServices { get; set; }
            = new HashSet<VehicleVehicleService>();
    }
}
