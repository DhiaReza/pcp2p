using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using pcp2p.Models;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;

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
        public static async Task InitBrandType(AppDbContext context)
        {

            // Check if data already exists
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // 1. Create Lookup Data
            var intel = new Brand 
            { 
                Name = "INTEL",
                NameCapitalized = "Intel",
                NameLowercase = "intel",
                NameUppercase = "INTEL" 
            }; // 01
            var amd = new Brand 
            { 
                Name = "AMD",
                NameCapitalized = "Amd",
                NameLowercase = "amd",
                NameUppercase = "AMD",
            }; //02
            var nvidia = new Brand 
            {
                Name = "NVIDIA" ,
                NameCapitalized = "Nvidia",
                NameLowercase = "nvidia",
                NameUppercase = "NVIDIA"
            }; // 01
            var cpuType = new HardwareType 
            { 
                Name = "CPU",
                NameLowercase = "cpu",
                NameUppercase = "CPU",
                NameCapitalized = "Cpu"
            }; // 02
            var gpuType = new HardwareType
            { 
                Name = "GPU",
                NameLowercase = "gpu",
                NameUppercase = "GPU",
                NameCapitalized = "Gpu"
            }; // 02
            
            // 2. Save to Database
            context.brands.AddRange(intel,amd,nvidia);
            context.hardwareTypes.AddRange(gpuType, cpuType);
            await context.SaveChangesAsync();
        }
        
        public static async Task SeedGPU(AppDbContext context, string gpufilepath)
        {
                        // Create dictionaries ONLY after confirming data exists
            var allBrands = await context.brands.ToDictionaryAsync(b => b.NameLowercase);
            var allTypes = await context.hardwareTypes.ToDictionaryAsync(t => t.NameLowercase);
                
            string jsonData = File.ReadAllText(gpufilepath);
            List<GPUSeedDTO> Gpu = JsonConvert.DeserializeObject<List<GPUSeedDTO>>(jsonData);

            foreach(var gpu in Gpu)
            {
                if (allBrands.TryGetValue(gpu.Brand.ToLower(), out var matchedBrand) &&
                    allTypes.TryGetValue(gpu.HardwareType.ToLower(), out var matchedType))
                {
                    var hardware = new Hardware
                    {
                        Name = gpu.Name,
                        MSRP = gpu.MSRP,
                        ReleaseDate = DateOnly.ParseExact(gpu.ReleaseDate, "d-M-yyyy"),
                        TDP = gpu.Gpu.TDP,
                        Brand = matchedBrand,
                        HardwareType = matchedType,
                        Gpu = new Gpu
                        {
                            Generation = gpu.Generation,
                            Architecture = gpu.Architecture,
                            Vram = gpu.Gpu.Vram,
                            BaseClock = gpu.Gpu.BaseClock,
                            BoostClock = gpu.Gpu.BoostClock,
                            GameClock = gpu.Gpu.GameClock
                        }
                    };
                    context.Hardwares.Add(hardware);
                }
            }
            await context.SaveChangesAsync();
        }

        public static async Task SeedCPU(AppDbContext context,string cpufilepath)
        {
            using (var reader = new StreamReader(cpufilepath))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.CultureInfo = CultureInfo.InvariantCulture;
                csv.Configuration.RegisterClassMap<CPUMap>();
                csv.Configuration.Delimiter = ",";

                var cpus = csv.GetRecords<CPUSeedDTO>();
                foreach(var cpu in cpus)
                {
                    var hardware = new Hardware
                    {
                        Name = cpu.Name,
                        MSRP =  cpu.MSRP,
                        ReleaseDate = DateOnly.ParseExact(cpu.ReleaseDate, "d-M-yyyy"),
                        TDP = cpu.TDP,
                        HardwareType = await context.hardwareTypes.Where(b => b.Name == "CPU").FirstOrDefaultAsync(),
                        Brand = await context.brands.Where(b => b.Name == cpu.Brand).FirstOrDefaultAsync(),
                        Cpu = new Cpu
                        {
                            CodeName = cpu.Codename,
                            Generation = cpu.Generation,
                            Socket = cpu.Socket,
                            CoreCount = cpu.CoreCount,
                            ThreadCount = cpu.ThreadCount,
                            BaseClock = cpu.BaseClock,
                            TurboClock = cpu.TurboClock,
                            L1_Cache = cpu.L1Cache,
                            L2_Cache = cpu.L2Cache,
                            L3_Cache = cpu.L3Cache
                        }
                    };
                    context.Add(hardware);
                }
            }
            await context.SaveChangesAsync();
        }

        public static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
        {
            // Resolve the Managers from the Service Provider
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            string adminUsername = "AdminAdmin";
            string adminPassword = "AdminAdmin0!";

            var admin = new IdentityUser 
            { 
                UserName = adminUsername, 
            };
                
            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
            else
            {
                // Log the errors so you know why it failed
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"User creation failed: {error.Description}");
                }
            }
        }  
    }
}