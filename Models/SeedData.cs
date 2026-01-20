using pcp2p.Models;

namespace pcp2p
{
    public class SeedData
    {
        private readonly ILogger _logger;
        private readonly AppDbContext _context;

        public SeedData(ILogger<SeedData> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public static void Initialize(AppDbContext context)
        {
            // Check if data already exists
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // 1. Create Lookup Data
            var intel = new Brand { Name = "Intel" }; // 01
            var amd = new Brand {Name = "AMD"}; //02
            var nvidia = new Brand { Name = "NVIDIA" }; // 03
            var cpuType = new HardwareType { Name = "CPU" }; // 01
            var gpuType = new HardwareType { Name = "GPU" }; // 02

            // 2. Create Hardware with specific sub-types
            var i9 = new Hardware
            {
                Name = "Core i9-13900K",
                Generation = "13th Gen",
                MSRP = 589.00m,
                ReleaseDate = new DateTime(2022, 10, 20),
                Brand = intel,
                HardwareType = cpuType,
                Cpu = new Cpu 
                { 
                    CoreCount = 24, 
                    ThreadCount = 32, 
                    BaseClock = 3000, 
                    BoostClock = 5800, 
                    TDP = 125 
                }
            };

            var rtx4090 = new Hardware
            {
                Name = "GeForce RTX 4090",
                Generation = "Ada Lovelace",
                MSRP = 1599.00m,
                ReleaseDate = new DateTime(2022, 10, 12),
                Brand = nvidia,
                HardwareType = gpuType,
                Gpu = new Gpu 
                { 
                    Vram = 24, 
                    BaseClock = 2230, 
                    BoostClock = 2520, 
                    GameClock = 2520, 
                    TDP = 450 
                }
            };

            var rx6800xt = new Hardware
            {
                Name = "Radeon RX 6800 XT",
                Generation = "NAVI II",
                MSRP = 649.00m,
                ReleaseDate = new DateTime(2020, 10, 28),
                Brand = amd,
                HardwareType = gpuType,
                Gpu = new Gpu 
                { 
                    Vram = 16, 
                    BaseClock = 1825, 
                    BoostClock = 2250, 
                    GameClock = 2015, 
                    TDP = 300
                }
            };
            
            // 3. Save to Database
            context.Hardwares.AddRange(i9, rtx4090, rx6800xt);
            context.SaveChanges();
        }
        
    }
}