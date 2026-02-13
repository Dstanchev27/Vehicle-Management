namespace VMAPP.Web.Models.VehicleServiceCars
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using VMAPP.Common;

    public class ServiceRecordViewModel
    {
        public int ServiceRecordId { get; set; }

        public DateTime ServiceDate { get; set; }

        public string Description { get; set; } = null!;

        public decimal Cost { get; set; }
    }
}
