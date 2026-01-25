using Microsoft.EntityFrameworkCore;

namespace VMAPP.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Models.Vehicle> Vehicles { get; set; } = null!;
        public DbSet<Models.ServiceRecord> ServiceRecords { get; set; } = null!;
        public DbSet<Models.VehicleService> VechicleServices { get; set; } = null!;
        public DbSet<Models.VehicleVechicleService> VehicleVechicleServices { get; set; } = null!;
    }
}
