namespace VMAPP.Web.Models.InsuranceModels
{
    using System.ComponentModel.DataAnnotations;

    public class EditInsuranceCompanyViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "City is required")]
        [MaxLength(100)]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(200)]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000)]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; } = null!;

        public DateTime CreatedOn { get; set; }
    }
}
