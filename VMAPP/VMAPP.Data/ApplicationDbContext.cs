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
            modelBuilder.Entity<Models.Vehicle>()
                .HasIndex(v => v.VIN)
                .IsUnique();

            modelBuilder.Entity<ServiceRecord>(entity =>
            {
                entity.Property(p => p.Cost)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.HasOne(sr => sr.Vehicle)
                    .WithMany(v => v.ServiceRecords)
                    .HasForeignKey(sr => sr.VehicleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sr => sr.VehicleService)
                    .WithMany(vs => vs.ServiceRecords)
                    .HasForeignKey(sr => sr.VehicleServiceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Models.Vehicle> Vehicles { get; set; } = null!;
        public DbSet<Models.ServiceRecord> ServiceRecords { get; set; } = null!;
        public DbSet<Models.VehicleService> VehicleServices { get; set; } = null!;
    }
}
