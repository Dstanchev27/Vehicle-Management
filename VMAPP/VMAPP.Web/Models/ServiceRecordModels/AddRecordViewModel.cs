namespace VMAPP.Web.Models.ServiceRecordModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using VMAPP.Common;

    public class AddRecordViewModel
    {
        public int ServiceRecordId { get; set; }
        public int VehicleId { get; set; }

        public string Description { get; set; } = null!;

        public decimal Cost { get; set; }
    }
}
