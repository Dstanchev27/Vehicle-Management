using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VMAPP.Data.Models;

namespace VMAPP.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
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

                entity.HasOne(sr => sr.CreatedBy)
                    .WithMany(u => u.CreatedServiceRecords)
                    .HasForeignKey(sr => sr.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InsurancePolicy>(entity =>
            {
                entity.HasOne(ip => ip.Vehicle)
                    .WithMany()
                    .HasForeignKey(ip => ip.VehicleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ip => ip.InsuranceCompany)
                    .WithMany(ic => ic.InsurancePolicies)
                    .HasForeignKey(ip => ip.InsuranceCompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ip => ip.CreatedBy)
                    .WithMany(u => u.CreatedInsurancePolicies)
                    .HasForeignKey(ip => ip.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ip => ip.ModifiedBy)
                    .WithMany()
                    .HasForeignKey(ip => ip.ModifiedById)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InsuranceCompany>(entity =>
            {
                entity.HasOne(ic => ic.CreatedBy)
                    .WithMany(u => u.CreatedInsuranceCompanies)
                    .HasForeignKey(ic => ic.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ic => ic.ModifiedBy)
                    .WithMany()
                    .HasForeignKey(ic => ic.ModifiedById)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InsuranceClaim>(entity =>
            {
                entity.Property(p => p.Amount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.HasOne(ic => ic.InsurancePolicy)
                    .WithMany(ip => ip.Claims)
                    .HasForeignKey(ic => ic.InsurancePolicyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Vehicle> Vehicles { get; set; } = null!;
        public DbSet<ServiceRecord> ServiceRecords { get; set; } = null!;
        public DbSet<VehicleService> VehicleServices { get; set; } = null!;
        public DbSet<InsuranceCompany> InsuranceCompanies { get; set; } = null!;
        public DbSet<InsurancePolicy> InsurancePolicies { get; set; } = null!;
        public DbSet<InsuranceClaim> InsuranceClaims { get; set; } = null!;
    }
}
