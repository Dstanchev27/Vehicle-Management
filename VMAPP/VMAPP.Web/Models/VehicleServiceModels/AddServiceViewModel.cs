namespace VMAPP.Web.Models.VehicleServiceModels
{
    using System.ComponentModel.DataAnnotations;

    public class AddServiceViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500)]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        public string Phone { get; set; } = null!;

        public DateTime CreatedOn { get; set; }
    }
}
