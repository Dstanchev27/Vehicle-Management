using System.ComponentModel.DataAnnotations;

namespace VMAPP.Web.Models.VehicleServiceModels
{
    public class VehicleServiceViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public DateTime CreatedOn { get; set; }
    }
}
