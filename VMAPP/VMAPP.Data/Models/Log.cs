namespace VMAPP.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Log
    {
        [Key]
        public int Id { get; set; }

        public string? Message { get; set; }

        public string? MessageTemplate { get; set; }

        [MaxLength(128)]
        public string? Level { get; set; }

        public DateTime TimeStamp { get; set; }

        public string? Exception { get; set; }

        // Replaces Properties (as per columnOptions.Store config)
        public string? LogEvent { get; set; }

        // Custom additional column
        [MaxLength(200)]
        public string? Username { get; set; }
    }
}