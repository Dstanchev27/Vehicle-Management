using System.ComponentModel.DataAnnotations;

namespace VMAPP.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using VMAPP.Common;

    public class ServiceRecord
    {
        [Key]
        public int ServiceRecordId { get; set; }

        [DataType(DataType.Date)]
        public DateTime ServiceDate { get; set; }

        [MaxLength(GlobalConstant.ServiceRecordDescription)]
        public string Description { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal Cost { get; set; }

        // Navigation property
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;
    }
}
