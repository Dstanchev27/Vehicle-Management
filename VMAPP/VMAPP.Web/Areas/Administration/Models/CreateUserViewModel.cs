namespace VMAPP.Web.Areas.Administration.Models
{
    using System.ComponentModel.DataAnnotations;

    using VMAPP.Data.Models.Enums;

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

        public UserType UserType { get; set; }

        [Required]
        public string SelectedRole { get; set; } = null!;
    }
}
