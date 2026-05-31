using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace pcp2p.Models
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }

        public DbSet<Hardware> Hardwares {get;set;}
        public DbSet<Cpu> cpus {get;set;}
        public DbSet<Gpu> gpus {get;set;}
        public DbSet<Brand> brands {get;set;}
        public DbSet<Benchmark> benchmarks {get;set;}
        public DbSet<HardwareType> hardwareTypes {get;set;}
        public DbSet<TestSource>  testSource {get;set;}
        public DbSet<TestGraphic> testGraphics {get;set;}
        public DbSet<TestResolution> testResolutions {get;set;}
        public DbSet<TestSubject> testSubjects {get;set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // each hardware link to one CPU or GPU
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

            modelBuilder.Entity<TestSource>()
                .HasIndex(t => t.Name) // Good practice to index unique types like "Raster" or "Ray Tracing"
                .IsUnique();           // Ensures you can't have two rows with the same Type name

            modelBuilder.Entity<Benchmark>()
                .HasOne(b => b.TestSource)   // Each Benchmark has ONE TestType
                .WithMany()                // A TestType can have MANY Benchmarks (or zero, depending on your logic)
                .HasForeignKey(b => b.TestSourceId); // The link is stored in the 'TestTypeId' column
            
            modelBuilder.Entity<TestGraphic>()
                .HasIndex(t => t.Name)
                .IsUnique();

            modelBuilder.Entity<Benchmark>()
                .HasOne(b => b.TestGraphic)   // Each Benchmark has ONE TestType
                .WithMany()                // A TestType can have MANY Benchmarks (or zero, depending on your logic)
                .HasForeignKey(b => b.TestGraphicId); // The link is stored in the 'TestTypeId' column
            
            modelBuilder.Entity<TestResolution>()
                .HasIndex(t => t.Name)
                .IsUnique();
                
            modelBuilder.Entity<Benchmark>()
                .HasOne(b => b.TestResolution)   // Each Benchmark has ONE TestType
                .WithMany()                // A TestType can have MANY Benchmarks (or zero, depending on your logic)
                .HasForeignKey(b => b.TestResolutionId); // The link is stored in the 'TestTypeId' column

            modelBuilder.Entity<TestSubject>()
                .HasIndex(t => t.Name)
                .IsUnique();
                
            modelBuilder.Entity<Benchmark>()
                .HasOne(b => b.TestSubject)   // Each Benchmark has ONE TestType
                .WithMany()                // A TestType can have MANY Benchmarks (or zero, depending on your logic)
                .HasForeignKey(b => b.TestSubjectId); // The link is stored in the 'TestTypeId' column
            base.OnModelCreating(modelBuilder);
        }
    }
}
