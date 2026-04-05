namespace VMAPP.Web.Areas.Administration.Models
{
    using System.ComponentModel.DataAnnotations;

    using VMAPP.Data.Models.Enums;

    public class EditUserViewModel
    {
        public string Id { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string City { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        public UserType UserType { get; set; }

        [Required]
        public string SelectedRole { get; set; } = null!;
    }
}
