namespace VMAPP.Web.Models.ServiceRecordModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using VMAPP.Common;

    public class AddRecordViewModel
    {
        public int RecordId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        public int? ServiceId { get; set; }
        public string? ServiceName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ServiceDate { get; set; } = DateTime.UtcNow.Date;

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal RecordCost { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
