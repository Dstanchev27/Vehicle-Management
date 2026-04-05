namespace VMAPP.Web.Areas.Administration.Models
{
    using VMAPP.Data.Models.Enums;

    public class UserDetailsViewModel
    {
        public string Id { get; set; } = null!;
        public string? Email { get; set; }
        public string City { get; set; } = null!;
        public string Address { get; set; } = null!;
        public UserType UserType { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
