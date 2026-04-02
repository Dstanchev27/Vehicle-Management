using System.ComponentModel.DataAnnotations;
using VMAPP.Common;
using VMAPP.Data.Models.Base;
using VMAPP.Data.Models.Enums;

namespace VMAPP.Data.Models
{
    public class Vehicle : IAuditInfo, IDeletableEntity
    {
        [Key]
        public int VehicleId { get; set; }

        [Required]
        [StringLength(17, MinimumLength = 17)]
        [RegularExpression(GlobalConstant.VINRegex, ErrorMessage = "VIN must be 17 alphanumeric characters")]
        public string VIN { get; set; } = null!;

        [Required]
        [MaxLength(GlobalConstant.CarBrandLength)]
        public string CarBrand { get; set; } = null!;

        [Required]
        [MaxLength(GlobalConstant.CarModelLength)]
        public string CarModel { get; set; } = null!;

        [Required]
        [Range(1886, 2100)]
        public int CreatedOnYear { get; set; }

        [Required]
        [MaxLength(50)]
        public string Color { get; set; } = null!;

        [Required]
        public VehicleType VehicleType { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }

        public ICollection<ServiceRecord> ServiceRecords { get; set; } = new HashSet<ServiceRecord>();
    }
}
