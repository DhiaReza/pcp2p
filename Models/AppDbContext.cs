using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace pcp2p.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }

        public DbSet<Hardware> Hardwares {get;set;}
        public DbSet<Cpu> cpus {get;set;}
        public DbSet<Gpu> gpus {get;set;}
        public DbSet<Brand> brands {get;set;}
        public DbSet<Benchmark> benchmarks {get;set;}
        public DbSet<HardwareType> hardwareTypes {get;set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // CPU settings
            // set primary CPU key to hardware Id.
            modelBuilder.Entity<Cpu>()
            .HasKey(c => c.HardwareId);

            // one to one relationship with CPU 
            modelBuilder.Entity<Cpu>()
            .HasOne(c => c.Hardware)
            .WithOne(h => h.Cpu)
            .HasForeignKey<Cpu>(c => c.HardwareId)
            .OnDelete(DeleteBehavior.Cascade);

            // GPU settings
            // set primary GPU key to Hardware Id
            modelBuilder.Entity<Gpu>()
            .HasKey(g => g.HardwareId);

            // one to one with GPU
            modelBuilder.Entity<Gpu>()
            .HasOne(g => g.Hardware)
            .WithOne(h => h.Gpu)
            .HasForeignKey<Gpu>(g => g.HardwareId)
            .OnDelete(DeleteBehavior.Cascade);

            // each hardware link to one CPU or GPU

            // decimal precision for MSRP
            modelBuilder.Entity<Hardware>()
            .Property(h => h.MSRP)
            .HasPrecision(18,2);
            
            // one to many with benchmark(s)
            modelBuilder.Entity<Benchmark>()
            .HasOne(a => a.Hardware)
            .WithMany(b => b.Benchmarks)
            .OnDelete(DeleteBehavior.Cascade);
            

            // Brand Relationship (One-to-Many)
            modelBuilder.Entity<Hardware>()
                .HasOne(h => h.Brand)          // Each hardware has ONE Brand
                .WithMany(b => b.Hardwares)    // But one Brand has MANY Hardwares
                .HasForeignKey(h => h.BrandId);

            // HardwareType Relationship (One-to-Many)
            modelBuilder.Entity<Hardware>()
                .HasOne(h => h.HardwareType)   // Each hardware has ONE Type (e.g., CPU)
                .WithMany(h => h.Hardwares)    // But one Type has MANY Hardwares
                .HasForeignKey(h => h.HardwareTypeId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
