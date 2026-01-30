using Microsoft.EntityFrameworkCore;
using VMAPP.Data.Models;

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

            modelBuilder.Entity<Models.VehicleVehicleService>()
                .HasKey(vvs => new { vvs.VehicleId, vvs.VehicleServiceId });

            modelBuilder.Entity<Models.VehicleVehicleService>()
                .HasOne(vvs => vvs.VehicleService)
                .WithMany(vs => vs.VehicleVehicleServices)
                .HasForeignKey(vvs => vvs.VehicleServiceId);

            modelBuilder.Entity<Models.VehicleVehicleService>()
                .HasOne(vvs => vvs.Vehicle)
                .WithMany(v => v.VehicleVehicleServices)
                .HasForeignKey(vvs => vvs.VehicleId);

            modelBuilder.Entity<ServiceRecord>(entity =>
            {
                entity.Property(p => p.Cost)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Models.Vehicle> Vehicles { get; set; } = null!;
        public DbSet<Models.ServiceRecord> ServiceRecords { get; set; } = null!;
        public DbSet<Models.VehicleService> VehicleServices { get; set; } = null!;
        public DbSet<Models.VehicleVehicleService> VehicleVehicleServices { get; set; } = null!;
    }
}
