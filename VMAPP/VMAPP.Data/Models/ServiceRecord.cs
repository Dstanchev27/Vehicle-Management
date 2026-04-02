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
    using VMAPP.Data.Models.Base;

    public class ServiceRecord : IAuditInfo, IDeletableEntity
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

        public int VehicleId { get; set; }

        public int VehicleServiceId { get; set; }

        public string CreatedById { get; set; } = null!;

        public Vehicle Vehicle { get; set; } = null!;

        public VehicleService VehicleService { get; set; } = null!;

        public ApplicationUser CreatedBy { get; set; } = null!;

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
