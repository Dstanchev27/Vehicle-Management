using System.ComponentModel.DataAnnotations;

namespace VMAPP.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using VMAPP.Common;

    public class ServiceRecord
    {
        [Key]
        public int ServiceRecordId { get; set; }
        public DateTime ServiceDate { get; set; }
        [MaxLength(GlobalConstant.ServiceRecordDescription)]
        public string Description { get; set; } = null!;
        public decimal Cost { get; set; }

        // Navigation property
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;
    }
}
