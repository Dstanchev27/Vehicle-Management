using System.ComponentModel.DataAnnotations;

namespace VMAPP.Web.Models.ServiceRecordModels
{
    public class ServiceRecordFormViewModel
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int VehicleServiceId { get; set; }

        [Required(ErrorMessage = "Cost is required.")]
        [Range(0, 999999.99, ErrorMessage = "Cost must be between 0 and 999,999.99.")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1500, MinimumLength = 3,
            ErrorMessage = "Description must be between 3 and 1500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Service date is required.")]
        public DateTime ServiceDate { get; set; } = DateTime.Today;
    }

    public class DeleteServiceRecordRequest
    {
        public int Id { get; set; }
    }
}