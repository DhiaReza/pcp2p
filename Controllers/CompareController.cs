using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using pcp2p.Models;
using System.Linq.Expressions;

namespace pcp2p.Controllers
{
    public class CompareController : Controller
    {
        private readonly ILogger<CompareController> _logger;
        private readonly AppDbContext _context;

        public int pageSize = 10;
        public CompareController(ILogger<CompareController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public ActionResult Index()
        {
            return View();
        }

        // GPU Action - shows GPU selection interface
        public async Task<IActionResult> Gpu()
        {
            ViewBag.HardwareType = "GPU";
            
            // Load all GPUs for selection
            var gpuType = await _context.hardwareTypes.FirstOrDefaultAsync(x => x.Name == "GPU");
            if (gpuType != null)
            {
                var allGpus = await _context.Hardwares
                    .Include(h => h.Brand)
                    .Where(h => h.HardwareTypeId == gpuType.Id)
                    .OrderBy(h => h.Name)
                    .ToListAsync();
                
                ViewBag.GPUs = allGpus;
            }
            
            // Load resolutions and graphics settings
            ViewBag.Resolutions = await _context.testResolutions.ToListAsync();
            ViewBag.Graphics = await _context.testGraphics.ToListAsync();
            
            return View("SelectHardware");
        }

        // CPU Action - shows CPU selection interface
        public async Task<IActionResult> Cpu()
        {
            ViewBag.HardwareType = "CPU";
            
            // Load all CPUs for selection
            var cpuType = await _context.hardwareTypes.FirstOrDefaultAsync(x => x.Name == "CPU");
            if (cpuType != null)
            {
                var allCpus = await _context.Hardwares
                    .Include(h => h.Brand)
                    .Where(h => h.HardwareTypeId == cpuType.Id)
                    .OrderBy(h => h.Name)
                    .ToListAsync();
                
                ViewBag.CPUs = allCpus;
            }
            
            return View("SelectHardware");
        }

        // Main comparison method
        [HttpPost]
        public async Task<IActionResult> CompareHardware(
            List<int> selectedHardwareIds,
            string benchmarkType,
            int? resolutionId = null,
            int? graphicId = null)
        {
            try
            {
                if (selectedHardwareIds == null || !selectedHardwareIds.Any())
                {
                    ViewBag.Error = "Please select at least one hardware to compare.";
                    return View("Error");
                }

                var results = new List<HardwareBenchmarkDto>();
                
                foreach (var hardwareId in selectedHardwareIds)
                {
                    var hardware = await _context.Hardwares
                        .Include(h => h.Brand)
                        .Include(h => h.HardwareType)
                        .FirstOrDefaultAsync(h => h.Id == hardwareId);
                    
                    if (hardware == null) continue;
                    
                    // First try to get original benchmark from 2025 data
                    var benchmark = await GetOriginalBenchmarkAsync(hardwareId, benchmarkType, resolutionId, graphicId);
                    
                    bool isInterpolated = false;
                    
                    // If no original benchmark exists, interpolate for older hardware
                    if (benchmark == null)
                    {
                        benchmark = await InterpolateBenchmarkForOlderHardwareAsync(
                            hardwareId, benchmarkType, resolutionId, graphicId);
                        isInterpolated = benchmark != null;
                    }
                    
                    if (benchmark != null)
                    {
                        // Check if score is 0 or negative (means couldn't run)
                        bool couldNotRun = benchmark.Score <= 0;
                        
                        results.Add(new HardwareBenchmarkDto
                        {
                            Hardware = hardware,
                            Benchmark = benchmark,
                            IsInterpolated = isInterpolated,
                            CouldNotRun = couldNotRun
                        });
                    }
                }
                
                // Order by score (highest first) - but put "CouldNotRun" at the bottom
                results = results
                    .OrderByDescending(x => x.CouldNotRun ? -1 : x.Benchmark?.Score ?? 0)
                    .ThenByDescending(x => x.Benchmark?.Score ?? 0)
                    .ToList();
                
                ViewBag.HardwareType = await GetHardwareTypeName(selectedHardwareIds.FirstOrDefault());
                ViewBag.BenchmarkType = benchmarkType;
                ViewBag.Resolution = resolutionId.HasValue ? 
                    await _context.testResolutions.Where(r => r.Id == resolutionId).Select(r => r.Name).FirstOrDefaultAsync() : "Default";
                ViewBag.GraphicSetting = graphicId.HasValue ? 
                    await _context.testGraphics.Where(g => g.Id == graphicId).Select(g => g.Name).FirstOrDefaultAsync() : "Default";
                
                return View("HardwareComparison", results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing hardware");
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        /// <summary>
        /// Get original benchmark from 2025 data first
        /// </summary>
        private async Task<Benchmark> GetOriginalBenchmarkAsync(
            int hardwareId,
            string benchmarkType,
            int? resolutionId = null,
            int? graphicId = null)
        {
            // First try to find by TestSubject Name (case insensitive)
            var testSubject = await _context.testSubjects
                .FirstOrDefaultAsync(ts => ts.Name.ToLower() == benchmarkType.ToLower());
            
            // If not found by name and benchmarkType is "rt" or "raytracing", try to find by ID 2
            if (testSubject == null && (benchmarkType.ToLower() == "rt" || benchmarkType.ToLower() == "raytracing"))
            {
                testSubject = await _context.testSubjects.FirstOrDefaultAsync(ts => ts.Id == 2);
            }
            
            // If still not found by name, try to find by ID if benchmarkType is a number
            if (testSubject == null && int.TryParse(benchmarkType, out int subjectId))
            {
                testSubject = await _context.testSubjects.FirstOrDefaultAsync(ts => ts.Id == subjectId);
            }
            
            if (testSubject == null)
                return null;
            
            var query = _context.benchmarks
                .Include(b => b.TestSubject)
                .Include(b => b.TestSource)
                .Include(b => b.TestResolution)
                .Include(b => b.TestGraphic)
                .Where(b => b.HardwareId == hardwareId && 
                           b.TestSourceId == 1 && // Original source only
                           b.TestSubjectId == testSubject.Id);
            
            if (resolutionId.HasValue)
                query = query.Where(b => b.TestResolutionId == resolutionId.Value);
            
            if (graphicId.HasValue)
                query = query.Where(b => b.TestGraphicId == graphicId.Value);
            
            var result = await query.FirstOrDefaultAsync();
            
            // If no result found and this is a GPU benchmark, try without resolution/graphic filters
            if (result == null && (resolutionId.HasValue || graphicId.HasValue))
            {
                query = _context.benchmarks
                    .Include(b => b.TestSubject)
                    .Include(b => b.TestSource)
                    .Include(b => b.TestResolution)
                    .Include(b => b.TestGraphic)
                    .Where(b => b.HardwareId == hardwareId && 
                               b.TestSourceId == 1 &&
                               b.TestSubjectId == testSubject.Id);
                
                result = await query.FirstOrDefaultAsync();
            }
            
            return result;
        }

        /// <summary>
        /// Interpolate benchmark for older hardware using 2025 data as reference
        /// </summary>
        private async Task<Benchmark> InterpolateBenchmarkForOlderHardwareAsync(
            int hardwareId,
            string benchmarkType,
            int? resolutionId = null,
            int? graphicId = null)
        {
            var hardware = await _context.Hardwares
                .Include(h => h.HardwareType)
                .Include(h => h.Brand)
                .FirstOrDefaultAsync(h => h.Id == hardwareId);
            
            if (hardware == null)
                return null;
            
            // Get hardware release year
            int hardwareYear = GetHardwareReleaseYear(hardware);
            
            // Only interpolate for hardware from 2023 and below that doesn't have 2025 data
            if (hardwareYear > 2023)
                return null;
            
            // Find the closest modern equivalent GPU (same tier, same brand)
            var modernEquivalent = await FindModernEquivalentAsync(hardware, benchmarkType);
            
            if (modernEquivalent == null)
                return null;
            
            // Get the modern hardware's benchmark score
            var modernBenchmark = await GetOriginalBenchmarkAsync(
                modernEquivalent.Id, benchmarkType, resolutionId, graphicId);
            
            if (modernBenchmark == null)
                return null;
            
            // Calculate interpolated score based on generational difference
            int modernYear = GetHardwareReleaseYear(modernEquivalent);
            int yearDifference = modernYear - hardwareYear;
            
            // Performance degradation factor (roughly 15-20% per generation going backwards)
            float interpolatedScore = modernBenchmark.Score;
            for (int i = 0; i < yearDifference; i++)
            {
                interpolatedScore = interpolatedScore / 1.15f; // 15% decrease per generation
            }
            
            // Ensure score is reasonable
            interpolatedScore = Math.Max(interpolatedScore, modernBenchmark.Score * 0.2f);
            interpolatedScore = Math.Min(interpolatedScore, modernBenchmark.Score * 0.9f);
            
            // Get or create TestSubject
            var testSubject = await _context.testSubjects
                .FirstOrDefaultAsync(ts => ts.Name.ToLower() == benchmarkType.ToLower());
            
            if (testSubject == null && (benchmarkType.ToLower() == "rt" || benchmarkType.ToLower() == "raytracing"))
            {
                testSubject = await _context.testSubjects.FirstOrDefaultAsync(ts => ts.Id == 2);
            }
            
            if (testSubject == null && int.TryParse(benchmarkType, out int subjectId))
            {
                testSubject = await _context.testSubjects.FirstOrDefaultAsync(ts => ts.Id == subjectId);
            }
            
            if (testSubject == null)
            {
                testSubject = new TestSubject { Name = benchmarkType };
                _context.testSubjects.Add(testSubject);
                await _context.SaveChangesAsync();
            }
            
            // Create interpolated benchmark
            var interpolatedBenchmark = new Benchmark
            {
                Name = $"{hardware.Name} - {benchmarkType} (Interpolated)",
                Date = DateTime.Now.Year,
                TestSourceId = 2, // Interpolated source
                HardwareId = hardwareId,
                TestSubjectId = testSubject.Id,
                Score = interpolatedScore,
                TestResolutionId = resolutionId ?? 1,
                TestGraphicId = graphicId,
            };
            
            // Save to database for future use
            _context.benchmarks.Add(interpolatedBenchmark);
            await _context.SaveChangesAsync();
            
            return interpolatedBenchmark;
        }

        /// <summary>
        /// Find modern equivalent hardware (same tier, same brand)
        /// </summary>
        private async Task<Hardware> FindModernEquivalentAsync(Hardware oldHardware, string benchmarkType)
        {
            var allHardware = await GetHardwareByType(oldHardware.HardwareType.Name);
            
            // Get hardware from 2023-2025
            var modernHardware = new List<Hardware>();
            foreach (var hw in allHardware)
            {
                int year = GetHardwareReleaseYear(hw);
                if (year >= 2023 && year <= 2025)
                {
                    // Check if it has original benchmarks
                    var testSubject = await GetTestSubjectForBenchmarkType(benchmarkType);
                    if (testSubject != null)
                    {
                        var hasBenchmark = await _context.benchmarks
                            .AnyAsync(b => b.HardwareId == hw.Id && 
                                          b.TestSourceId == 1 &&
                                          b.TestSubjectId == testSubject.Id);
                        
                        if (hasBenchmark)
                        {
                            modernHardware.Add(hw);
                        }
                    }
                }
            }
            
            // Find the most similar hardware (same tier, same brand)
            var oldTier = DetermineHardwareTier(oldHardware);
            var oldBrand = oldHardware.Brand?.Name?.ToLower() ?? "";
            
            var bestMatch = modernHardware
                .Where(h => DetermineHardwareTier(h) == oldTier)
                .OrderBy(h => h.Brand?.Name?.ToLower() == oldBrand ? 0 : 1) // Same brand first
                .ThenBy(h => Math.Abs(GetHardwareReleaseYear(h) - 2024)) // Closest to 2024
                .FirstOrDefault();
            
            // If no exact tier match, find closest tier
            if (bestMatch == null)
            {
                var tiers = new[] { "enthusiast", "high-end", "mid-range", "entry", "budget" };
                var oldTierIndex = Array.IndexOf(tiers, oldTier);
                
                bestMatch = modernHardware
                    .Select(h => new { Hardware = h, TierIndex = Array.IndexOf(tiers, DetermineHardwareTier(h)) })
                    .Where(x => x.TierIndex >= 0)
                    .OrderBy(x => Math.Abs(x.TierIndex - oldTierIndex))
                    .ThenBy(x => x.Hardware.Brand?.Name?.ToLower() == oldBrand ? 0 : 1)
                    .Select(x => x.Hardware)
                    .FirstOrDefault();
            }
            
            return bestMatch;
        }

        /// <summary>
        /// Get TestSubject for benchmark type
        /// </summary>
        private async Task<TestSubject> GetTestSubjectForBenchmarkType(string benchmarkType)
        {
            // Try by name first
            var testSubject = await _context.testSubjects
                .FirstOrDefaultAsync(ts => ts.Name.ToLower() == benchmarkType.ToLower());
            
            // If not found and it's raytracing/RT, try ID 2
            if (testSubject == null && (benchmarkType.ToLower() == "rt" || benchmarkType.ToLower() == "raytracing"))
            {
                testSubject = await _context.testSubjects.FirstOrDefaultAsync(ts => ts.Id == 2);
            }
            
            // If still not found, try parsing as ID
            if (testSubject == null && int.TryParse(benchmarkType, out int subjectId))
            {
                testSubject = await _context.testSubjects.FirstOrDefaultAsync(ts => ts.Id == subjectId);
            }
            
            return testSubject;
        }

        /// <summary>
        /// Determine hardware tier based on naming
        /// </summary>
        private string DetermineHardwareTier(Hardware hardware)
        {
            if (hardware == null) return "budget";
            
            string name = hardware.Name.ToLower();
            
            // NVIDIA GPUs
            if (name.Contains("5090")) return "enthusiast";
            if (name.Contains("4090") || name.Contains("4080")) return "enthusiast";
            if (name.Contains("4070")) return "high-end";
            if (name.Contains("4060")) return "mid-range";
            if (name.Contains("4050")) return "entry";
            
            // AMD GPUs
            if (name.Contains("7900 xtx") || name.Contains("7900 xt")) return "enthusiast";
            if (name.Contains("7800 xt")) return "high-end";
            if (name.Contains("7700 xt") || name.Contains("7600 xt")) return "mid-range";
            if (name.Contains("7500")) return "entry";
            
            // Older NVIDIA
            if (name.Contains("3090") || name.Contains("3080")) return "enthusiast";
            if (name.Contains("3070")) return "high-end";
            if (name.Contains("3060")) return "mid-range";
            if (name.Contains("3050")) return "entry";
            
            // Older AMD
            if (name.Contains("6950") || name.Contains("6900") || name.Contains("6800")) return "enthusiast";
            if (name.Contains("6750") || name.Contains("6700")) return "high-end";
            if (name.Contains("6650") || name.Contains("6600")) return "mid-range";
            
            // CPUs
            if (name.Contains("i9") || name.Contains("ryzen 9")) return "enthusiast";
            if (name.Contains("i7") || name.Contains("ryzen 7")) return "high-end";
            if (name.Contains("i5") || name.Contains("ryzen 5")) return "mid-range";
            if (name.Contains("i3") || name.Contains("ryzen 3")) return "entry";
            
            return "budget";
        }

        /// <summary>
        /// Get hardware release year
        /// </summary>
        private int GetHardwareReleaseYear(Hardware hardware)
        {
            if (hardware.ReleaseDate != default && hardware.ReleaseDate.Year > 2000)
            {
                return hardware.ReleaseDate.Year;
            }
            
            var name = hardware.Name;
            
            // 2025 GPUs
            if (name.Contains("5090") || name.Contains("5080") || name.Contains("5070")) return 2025;
            
            // 2023-2024 GPUs
            if (name.Contains("4090") || name.Contains("4080") || name.Contains("4070") || name.Contains("4060")) return 2023;
            if (name.Contains("7900") || name.Contains("7800") || name.Contains("7700") || name.Contains("7600")) return 2023;
            
            // 2020-2022 GPUs
            if (name.Contains("3090") || name.Contains("3080") || name.Contains("3070") || name.Contains("3060")) return 2020;
            if (name.Contains("6950") || name.Contains("6900") || name.Contains("6800") || name.Contains("6700")) return 2020;
            
            // 2018-2019 GPUs
            if (name.Contains("2080") || name.Contains("2070") || name.Contains("2060")) return 2018;
            if (name.Contains("5700") || name.Contains("5600")) return 2019;
            
            // 2016-2017 GPUs
            if (name.Contains("1080") || name.Contains("1070") || name.Contains("1060")) return 2016;
            
            // CPUs
            if (name.Contains("14900") || name.Contains("14700") || name.Contains("14600")) return 2024;
            if (name.Contains("13900") || name.Contains("13700") || name.Contains("13600")) return 2023;
            if (name.Contains("12900") || name.Contains("12700") || name.Contains("12600")) return 2021;
            if (name.Contains("11900") || name.Contains("11700") || name.Contains("11600")) return 2020;
            
            if (name.Contains("9950") || name.Contains("9900") || name.Contains("9700") || name.Contains("9600")) return 2024;
            if (name.Contains("7950") || name.Contains("7900") || name.Contains("7800") || name.Contains("7700")) return 2023;
            if (name.Contains("5950") || name.Contains("5900") || name.Contains("5800") || name.Contains("5600")) return 2020;
            
            return 2020; // Default fallback
        }

        /// <summary>
        /// Get all hardware by type
        /// </summary>
        private async Task<List<Hardware>> GetHardwareByType(string hardwareType)
        {
            var hardwareTypeEntity = await _context.hardwareTypes
                .FirstOrDefaultAsync(x => x.Name.ToLower() == hardwareType.ToLower());
            
            if (hardwareTypeEntity == null)
                return new List<Hardware>();
            
            return await _context.Hardwares
                .Include(h => h.HardwareType)
                .Include(h => h.Brand)
                .Where(x => x.HardwareTypeId == hardwareTypeEntity.Id)
                .ToListAsync();
        }

        private async Task<string> GetHardwareTypeName(int hardwareId)
        {
            var hardware = await _context.Hardwares
                .Include(h => h.HardwareType)
                .FirstOrDefaultAsync(h => h.Id == hardwareId);
            
            return hardware?.HardwareType?.Name ?? "Hardware";
        }

        public List<Hardware> GetHardware(string hardwareType, string searchString, string sortOrder, int currentPage)
        {
            var hardwareTypeEntity = _context.hardwareTypes
                .FirstOrDefault(x => x.Name == hardwareType);

            if (hardwareTypeEntity == null)
            {
                throw new Exception("Hardware type not found.");
            }

            int hw_id = hardwareTypeEntity.Id;

            IQueryable<Hardware> hardware;

            hardware = _context.Hardwares
                .Where(x => x.HardwareTypeId == hw_id);

            return hardware.ToList();
        }
    }

    // Helper DTOs
    public class HardwareBenchmarkDto
    {
        public Hardware Hardware { get; set; }
        public Benchmark Benchmark { get; set; }
        public bool IsInterpolated { get; set; }
        public bool CouldNotRun { get; set; }
    }
}