using System.ComponentModel.DataAnnotations;
using VMAPP.Data.Models.Enums;

namespace VMAPP.Web.Models.VehicleViewModels
{
    public class AddVehicleViewModel
    {
        [Required(ErrorMessage = "VIN is required.")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "VIN must be exactly 17 characters.")]
        [RegularExpression(@"^[A-HJ-NPR-Z0-9]{17}$", ErrorMessage = "VIN can only contain uppercase letters (except I, O, Q) and digits.")]
        [Display(Name = "VIN")]
        public string VIN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Brand is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Brand must be between 2 and 50 characters.")]
        [Display(Name = "Brand")]
        public string CarBrand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Model must be between 1 and 50 characters.")]
        [Display(Name = "Model")]
        public string CarModel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Year is required.")]
        [Range(1886, 2100, ErrorMessage = "Year must be between 1886 and 2100.")]
        [Display(Name = "Year")]
        public int CreatedOnYear { get; set; }

        [Required(ErrorMessage = "Color is required.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Color must be between 2 and 30 characters.")]
        [Display(Name = "Color")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vehicle type is required.")]
        [EnumDataType(typeof(VehicleType), ErrorMessage = "Please select a valid vehicle type.")]
        [Display(Name = "Vehicle Type")]
        public VehicleType VehicleType { get; set; }
    }
}