using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using pcp2p.Models;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;
using Microsoft.VisualBasic;
using System.Runtime.CompilerServices;

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
        public static async Task SeedBrandAndType(AppDbContext context)
        {

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

        public static async Task SeedGPUBenchmark(int year,string subject, string pathfile, AppDbContext context)
        {
            // read csv file
            var reader = new StreamReader(pathfile);
            // create csv reader object
            var csv = new CsvReader(reader);
            //use mapping
            csv.Configuration.RegisterClassMap<GPU_Benchmark_Map>();
            //csv reader object to list
            var gpubenchmark = csv.GetRecords<GPU_Benchmark>().ToList();

            // declare repated use variable
            var testSource = await context.testSource.Where(b => b.Name == "original").Select(x => x.Id).FirstOrDefaultAsync();
            var testSubject = await context.testSubjects.Where(b => b.Name == subject).Select(x => x.Id).FirstOrDefaultAsync();
            var preset1080pmed = await context.testPresets.Where(b => b.Name == "1080p Medium").Select(x => x.Id).FirstOrDefaultAsync();
            var preset1080pult = await context.testPresets.Where(b => b.Name == "1080p Ultra").Select(x => x.Id).FirstOrDefaultAsync();
            var preset1440pult = await context.testPresets.Where(b => b.Name == "1440p Ultra").Select(x => x.Id).FirstOrDefaultAsync();
            var preset4kult= await context.testPresets.Where(b => b.Name == "4K Ultra").Select(x => x.Id).FirstOrDefaultAsync();
            
            // inserting benchmark for each testpreset
            foreach(var benchmark in gpubenchmark)
            {
                int hwid = await context.Hardwares.Where(b => b.Name == benchmark.Name).Select(x => x.Id).FirstOrDefaultAsync();
                // 1080p medium
                context.benchmarks.Add(new Benchmark
                {
                    HardwareId = hwid,
                    Date = year,
                    TestSourceId = testSource,
                    TestSubjectId = testSubject,
                    Score = benchmark.Medium1080p,
                    TestPresetId = preset1080pmed,
                });

                //1080p Ultra
                context.benchmarks.Add(new Benchmark
                {
                    HardwareId = hwid,
                    Date = year,
                    TestSourceId = testSource,
                    TestSubjectId = testSubject,
                    Score = benchmark.Ultra1080p,
                    TestPresetId = preset1080pult,
                });

                //1440p Ultra
                context.benchmarks.Add(new Benchmark
                {
                    HardwareId = hwid,
                    Date = year,
                    TestSourceId = testSource,
                    TestSubjectId = testSubject,
                    Score = benchmark.Ultra1440p,
                    TestPresetId = preset1440pult,
                });

                //1440p Ultra
                context.benchmarks.Add(new Benchmark
                {
                    HardwareId = hwid,
                    Date = year,
                    TestSourceId = testSource,
                    TestSubjectId = testSubject,
                    Score = benchmark.Ultra4K,
                    TestPresetId = preset4kult,
                });
                Console.WriteLine($"Added {benchmark.Name} to Benchmark");
            }
            await context.SaveChangesAsync();
            Console.WriteLine($"Finished adding {year} {subject} benchmark entry");
        }

        public static async Task SeedCPUBenchmark(AppDbContext context, string pathfile, string subject, int year)
        {
                        // read csv file
            var reader = new StreamReader(pathfile);
            // create csv reader object
            var csv = new CsvReader(reader);
            //use mapping
            csv.Configuration.RegisterClassMap<CPU_Benchmark_Map>();
            //csv reader object to list
            var cpubenchmark = csv.GetRecords<CPU_Benchmark>().ToList();

            // declare repated use variable
            int testSubjectId = await context.testSubjects.Where(b => b.Name == subject).Select(x => x.Id).FirstOrDefaultAsync();
            var testSourceid = await context.testSource.Where(b => b.Name == "original").Select(b => b.Id).FirstOrDefaultAsync();

            foreach(var cpu in cpubenchmark)
            {
                int hwid = await context.Hardwares.Where(b => b.Name == cpu.Name).Select(x => x.Id).FirstOrDefaultAsync();
                if (hwid == 0)
                {
                    Console.WriteLine($"Hardware not found: {cpu.Name}");
                    continue;
                }
                context.benchmarks.Add(new Benchmark
                {
                    HardwareId = hwid,
                    Date = year,
                    TestSubjectId = testSubjectId,
                    TestSourceId = testSourceid,
                    Score = cpu.FPS,
                });
                Console.WriteLine($"Successfully added {cpu.Name} to benchmark");
            }
            await context.SaveChangesAsync();
            Console.WriteLine($"Finished adding {year} {subject} benchmark entries");
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
        public static async Task SeedGraphic(AppDbContext context)
        {
            var medium = new TestGraphic
            {
                Name = "medium"
            };
            var low = new TestGraphic
            {
                Name = "low"
            };
            var high = new TestGraphic
            {
                Name = "high"
            };
            var ultra = new TestGraphic
            {
                Name = "ultra"
            };
            context.AddRange(low,medium,high,ultra);
            await context.SaveChangesAsync();
        }
        public static async Task SeedResolution(AppDbContext context)
        {
            var fhd = new TestResolution
            {
                Name = "1080p"
            };
            var qhd = new TestResolution
            {
                Name = "1440p"
            };
            var uhd = new TestResolution
            {
                Name = "4K"
            };
            context.AddRange(fhd,qhd,uhd);
            await context.SaveChangesAsync();
        }
        public static async Task SeedSource(AppDbContext context)
        {
            var original = new TestSource
            {
                Name = "original"
            };
            var interpolated = new TestSource
            {
                Name = "interpolated"
            };
            context.AddRange(original,interpolated);
            await context.SaveChangesAsync();
        }
        public static async Task SeedTestSubject(AppDbContext context)
        {
            var raster = new TestSubject
            {
                Name = "raster"
            };
            var rt = new TestSubject
            {
                Name = "raytracing"
            };
            var gaming = new TestSubject
            {
                Name = "gaming"
            };
            var single = new TestSubject
            {
                Name = "singlecore"
            };
            var multi = new TestSubject
            {
                Name = "multicore"
            };
            context.AddRange(raster,rt,gaming,single,multi);
            await context.SaveChangesAsync();
        }
        public static async Task SeedTestPreset(AppDbContext context)
        {
            var med1080p = new TestPreset
            {
                Name = "1080p Medium"
            };
            var ult1080p = new TestPreset
            {
                Name = "1080p Ultra"
            };
            var ult1440p = new TestPreset
            {
                Name = "1440p Ultra"
            };
            var ult4k = new TestPreset
            {
                Name = "4K Ultra"
            };
            context.testPresets.AddRange(med1080p, ult1080p, ult1440p, ult4k);
            await context.SaveChangesAsync();
            Console.WriteLine("TSETESFFE LAUNCEDdf");
        }
    }
}