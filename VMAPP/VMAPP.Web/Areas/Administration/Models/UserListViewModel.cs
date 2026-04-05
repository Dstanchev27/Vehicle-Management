namespace VMAPP.Web.Areas.Administration.Models
{
    using VMAPP.Data.Models.Enums;

    public class UserListViewModel
    {
        public string Id { get; set; } = null!;
        public string? Email { get; set; }
        public string City { get; set; } = null!;
        public string Address { get; set; } = null!;
        public UserType UserType { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
