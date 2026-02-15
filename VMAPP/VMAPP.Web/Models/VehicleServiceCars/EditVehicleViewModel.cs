using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using VMAPP.Data.Models.Enums;
using VMAPP.Web.Models.ServiceRecordModels;

namespace VMAPP.Web.Models.VehicleServiceCars
{
    public class EditVehicleViewModel
    {
        [Required]
        public int VehicleId { get; set; }

        [Required]
        [StringLength(100)]
        public string VIN { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CarBrand { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CarModel { get; set; } = string.Empty;

        [Range(1886, 2100)]
        public int? CreatedOnYear { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        public VehicleType VehicleType { get; set; }

        public int? ServiceId { get; set; }
        public string? ServiceName { get; set; }

        public List<AddRecordViewModel> ServiceRecords { get; set; } = new();
    }
}