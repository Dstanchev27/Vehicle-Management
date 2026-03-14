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
        [StringLength(17, MinimumLength = 17)]
        [RegularExpression(GlobalConstant.VINRegex, ErrorMessage = "VIN must be 17 alphanumeric characters")]
        public string VIN { get; set; } = null!;

        [Required]
        [MaxLength(GlobalConstant.CarBrandLength)]
        public  string CarBrand { get; set; } = null!;

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
        public  VehicleType VehicleType { get; set; }

        // Navigation properties
        public ICollection<ServiceRecord> ServiceRecords { get; set; } = new HashSet<ServiceRecord>();
    }
}
