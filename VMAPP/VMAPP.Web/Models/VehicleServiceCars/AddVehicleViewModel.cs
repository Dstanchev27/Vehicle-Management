namespace VMAPP.Web.Models.VehicleServiceCars
{
    using System.ComponentModel.DataAnnotations;
    using VMAPP.Common;
    using VMAPP.Data.Models;
    using VMAPP.Data.Models.Enums;

    public class AddVehicleViewModel
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int VehicleId { get; set; }

        [Required]
        [StringLength(17, MinimumLength = 17)]
        [RegularExpression(GlobalConstant.VINRegex, ErrorMessage = "VIN must be 17 alphanumeric characters")]
        public string VIN { get; set; } = string.Empty;

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

        public ICollection<ServiceRecordViewModel> ServiceRecord { get; set; } = new List<ServiceRecordViewModel>();
    }
}
