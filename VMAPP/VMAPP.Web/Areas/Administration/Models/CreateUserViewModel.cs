namespace VMAPP.Web.Areas.Administration.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        public string City { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public string SelectedRole { get; set; } = null!;
    }
}
