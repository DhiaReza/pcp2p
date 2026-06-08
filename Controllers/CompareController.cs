using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.VisualBasic;
using pcp2p.Models;
using System.ComponentModel;
using System.Formats.Asn1;
using System.Linq.Expressions;
using System.Net;

namespace pcp2p.Controllers
{
    public class CompareController : Controller
    {
        private readonly ILogger<CompareController> _logger;
        private readonly AppDbContext _context;
        public SelectHardwareDTO hw;
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
        // public async Task<IActionResult> Gpu()
        // {            
        //     SelectHardwareDTO select = new SelectHardwareDTO()
        //     {
        //         hardwares = _context.Hardwares.Where(b => b.HardwareType.Name == "gpu").ToList()
        //         testSubjects = _context.testSubjects.ToList();
        //     }
        //     return View("SelectHardware");
        // }

        // CPU Action - shows CPU selection interface
        // public async Task<IActionResult> Cpu()
        // {
            
        //     return View("SelectHardware");
        // }
        // public async Task<IActionResult> Gpu()  
        // {
        //     int hwtype = 1;
        //     var data = GetHardwareSelection(hwtype);
        //     return View("SelectHardware", data);
        // }
        public IActionResult SelectHardware(int hwtypeid)
        {
            hw = GetHardwareSelection(hwtypeid);

            return View(hw);
        }
        public async Task<IActionResult> HardwareComparison(
            List<int>? hardwareids,
            int presetid,
            int subjectid,
            int hwtypeid)
        {
            List<HardwareComparisonDTO> listScores = [];

            // Validation
            if (hardwareids == null || !hardwareids.Any())
            {
                ModelState.AddModelError("hardwares", "Please select at least one hardware!");
            }

            if (presetid <= 0)
            {
                ModelState.AddModelError("testPresets", "Please select a valid preset!");
            }

            if (subjectid <= 0)
            {
                ModelState.AddModelError("testSubjects", "Please select a valid category!");
            }

            if (!ModelState.IsValid)
            {
                var hw = GetHardwareSelection(hwtypeid);
                return View("SelectHardware", hw);
            }

            // ============================================
            // GPU ONLY LOGIC - Currently hwtypeid == 1 is GPU
            // TODO: When adding CPU, hwtypeid == 2 will be CPU
            // ============================================
            
            if (hwtypeid == 1) // GPU
            {
                // Check availability for all GPUs
                var availabilityResults = new Dictionary<int, BenchmarkAvailability>();
                bool hasOnly2022 = true;
                bool hasOnly2025 = true;
                bool hasBoth = false;

                foreach (int hwId in hardwareids)
                {
                    var availability = await GetBenchmarkAvailability(hwId, presetid, subjectid);
                    availabilityResults[hwId] = availability;
                    
                    if (availability == BenchmarkAvailability.Both) 
                    {
                        hasBoth = true;
                        hasOnly2022 = false;
                        hasOnly2025 = false;
                    }
                    else if (availability == BenchmarkAvailability.Only2022)
                    {
                        hasOnly2025 = false;
                    }
                    else if (availability == BenchmarkAvailability.Only2025)
                    {
                        hasOnly2022 = false;
                    }
                    else if (availability == BenchmarkAvailability.None)
                    {
                        ModelState.AddModelError("hardwares", $"GPU ID {hwId} has no benchmarks for the selected criteria");
                        var hw = GetHardwareSelection(hwtypeid);
                        return View("SelectHardware", hw);
                    }
                }

                double factor = 1.0;
                bool use2025Only = false;
                
                // Determine which scores to use for GPUs
                if (hasBoth || (hasOnly2025 && hasOnly2022))
                {
                    // Mix of GPU types - need to normalize using factor
                    factor = await GetGPUAnchorFactor(presetid, subjectid);
                    use2025Only = false;
                }
                else if (hasOnly2025)
                {
                    // All GPUs have only 2025 benchmarks
                    use2025Only = true;
                    factor = 1.0;
                }
                else if (hasOnly2022)
                {
                    // All GPUs have only 2022 benchmarks
                    use2025Only = false;
                    factor = 1.0;
                }

                // Calculate scores for each GPU
                foreach (int hwId in hardwareids)
                {
                    double baseScore;
                    var availability = availabilityResults[hwId];
                    
                    if (use2025Only || availability == BenchmarkAvailability.Only2025)
                    {
                        // Use 2025 score directly
                        baseScore = await _context.benchmarks
                            .Where(b => b.HardwareId == hwId &&
                                        b.TestPresetId == presetid &&
                                        b.TestSubjectId == subjectid &&
                                        b.Date == 2025)
                            .AverageAsync(b => (double?)b.Score) ?? 0;
                    }
                    else if (availability == BenchmarkAvailability.Both && hasOnly2022 == false && hasOnly2025 == false)
                    {
                        // Mixed scenario - use 2022 score with factor
                        double score2022 = await _context.benchmarks
                            .Where(b => b.HardwareId == hwId &&
                                        b.TestPresetId == presetid &&
                                        b.TestSubjectId == subjectid &&
                                        b.Date == 2022)
                            .AverageAsync(b => (double?)b.Score) ?? 0;
                        baseScore = score2022 * factor;
                    }
                    else // Only2022 or all have 2022
                    {
                        // Use 2022 score directly
                        baseScore = await _context.benchmarks
                            .Where(b => b.HardwareId == hwId &&
                                        b.TestPresetId == presetid &&
                                        b.TestSubjectId == subjectid &&
                                        b.Date == 2022)
                            .AverageAsync(b => (double?)b.Score) ?? 0;
                    }
                    
                    // Get price and hardware info
                    decimal price = await _context.Hardwares
                        .Where(b => b.Id == hwId)
                        .Select(b => b.MSRP)
                        .FirstOrDefaultAsync();
                        
                    var hardware = await _context.Hardwares
                        .FirstOrDefaultAsync(b => b.Id == hwId);
                        
                    var gpu = await _context.gpus
                        .Where(b => b.HardwareId == hwId)
                        .FirstOrDefaultAsync();



                    listScores.Add(new HardwareComparisonDTO
                    {
                        Hw = hardware,
                        Gpu = gpu,
                        Score = baseScore,
                        P2P = price > 0 ? baseScore / (double)price : 0,
                    });

                    double bestP2P = listScores.Max(b => b.P2P);

                    foreach (var item in listScores)
                    {
                        item.P2PPercent = (item.P2P / bestP2P) * 100;
                    }

                }
            }
            // ============================================
            // FUTURE CPU CODE - ADD CPU LOGIC HERE
            // ============================================
            // TODO: Add CPU logic when hwtypeid == 2
            // else if (hwtypeid == 2) // CPU
            // {
            //     // Similar logic as GPU but using CPU anchor factor
            //     // double factor = await GetCPUAnchorFactor(presetid, subjectid);
            //     // ... rest of CPU comparison logic
            // }
            
            ViewBag.Preset = await _context.testPresets
                .Where(b => b.Id == presetid)
                .Select(b => b.Name)
                .FirstOrDefaultAsync();
                
            ViewBag.Subject = await _context.testSubjects
                .Where(b => b.Id == subjectid)
                .Select(b => b.Name)
                .FirstOrDefaultAsync();
            ViewBag.HardwareId = await _context.hardwareTypes
                .Where(b => b.Id == hwtypeid)
                .Select(b => b.Id)
                .FirstOrDefaultAsync();

            listScores = listScores
                .OrderByDescending(b => b.P2P)
                .ToList();
            return View(listScores);
        }
        // get hardware selection data
        public SelectHardwareDTO GetHardwareSelection(int hwTypeId)
        {
            SelectHardwareDTO hw;
            if(hwTypeId == 1)
            {
                hw = new SelectHardwareDTO
                {
                    hwtypeid = hwTypeId,
                    hardwares = _context.Hardwares
                        .Where(b => b.HardwareTypeId == hwTypeId)
                        .Select(b => new HardwareOptions
                        {
                            Id = b.Id,
                            Name = b.Name,
                            Vram = b.Gpu.Vram,
                            MSRP = b.MSRP,
                            Generation = b.Gpu.Generation
                        })
                        .ToList(),

                    testSubjects = _context.testSubjects
                        .Where(b => b.Name == "raster" || b.Name == "raytracing")
                        .ToList(),

                    testPresets = _context.testPresets.ToList()
                };
            }
            else
            {
                hw = new SelectHardwareDTO
                {
                    hwtypeid = hwTypeId,
                    hardwares = _context.Hardwares
                        .Where(b => b.HardwareTypeId == hwTypeId)
                        .Select(b => new HardwareOptions
                        {
                            Id = b.Id,
                            Name = b.Name,
                            Vram = b.Gpu.Vram,
                            MSRP = b.MSRP,
                            Generation = b.Gpu.Generation
                        })
                        .ToList(),

                    testSubjects = _context.testSubjects
                        .Where(b => b.Name == "raster" || b.Name == "raytracing")
                        .ToList(),

                    testPresets = _context.testPresets.ToList()
                };
            }
            return hw;
        }

        // Performance Prediction Logic
        // get anchor factor score GPU(s) that has both 2025 and 2022 benchmark DONE
        // divide 2025 benchmark score with 2022 benchmark score DONE
        // do this with several gpus to get a precise factor score DONE
        // Multiple selected GPU with the factor
        // send to front

        // get anchor
        public async Task<double> GetGPUAnchorFactor(int presetid, int subjectid)
        {
            // Anchor GPUs
            int rtx4060id = await _context.Hardwares
                .Where(b => b.Name.Contains("4060"))
                .Select(b => b.Id)
                .FirstOrDefaultAsync();
                
            int rtx4090id = await _context.Hardwares
                .Where(b => b.Name.Contains("4090"))
                .Select(b => b.Id)
                .FirstOrDefaultAsync();
                
            int rtx4070id = await _context.Hardwares
                .Where(b => b.Name.Contains("4070"))
                .Select(b => b.Id)
                .FirstOrDefaultAsync();
                
            int rx7700id = await _context.Hardwares
                .Where(b => b.Name.Contains("7700"))
                .Select(b => b.Id)
                .FirstOrDefaultAsync();
                
            int rx7800id = await _context.Hardwares
                .Where(b => b.Name.Contains("7800"))
                .Select(b => b.Id)
                .FirstOrDefaultAsync();

            var anchorGpus = new List<int> { rtx4060id, rtx4090id, rtx4070id, rx7700id, rx7800id }
                .Where(id => id > 0) // Only include valid IDs
                .ToList();
            
            var factors = new List<double>();
            
            foreach(int gpuId in anchorGpus)
            {
                double currentFact = await GetFactor(gpuId, presetid, subjectid);
                if (currentFact > 0)
                    factors.Add(currentFact);
            }
            
            return factors.Any() ? factors.Average() : 1.0;
        }
        // ============================================
        // FUTURE CPU METHODS - ADD CPU CODE HERE
        // ============================================

        // TODO: Add CPU Anchor Method for when hwtypeid == 2
        // public async Task<double> GetCPUAnchorFactor(int presetid, int subjectid)
        // {
        //     // Define your CPU anchor IDs here
        //     // Example:
        //     // int cpuAnchor1Id = await _context.Hardwares
        //     //     .Where(b => b.Name.Contains("13900K"))
        //     //     .Select(b => b.Id)
        //     //     .FirstOrDefaultAsync();
        //     //     
        //     // var anchorCpus = new List<int> { cpuAnchor1Id, cpuAnchor2Id };
        //     // 
        //     // var factors = new List<double>();
        //     // 
        //     // foreach(int cpuId in anchorCpus)
        //     // {
        //     //     double currentFact = await GetFactor(cpuId, presetid, subjectid);
        //     //     if (currentFact > 0)
        //     //         factors.Add(currentFact);
        //     // }
        //     // 
        //     // return factors.Any() ? factors.Average() : 1.0;
        // }

        // ============================================
        // MAIN HARDWARE COMPARISON METHOD (GPU ONLY)
        // ============================================

        public async Task<double> GetFactor(int hwid, int presetid, int subjectid)
        {
            double score2022 = await _context.benchmarks
                .Where(b => b.HardwareId == hwid &&
                            b.TestPresetId == presetid &&
                            b.TestSubjectId == subjectid &&
                            b.Date == 2022)
                .AverageAsync(b => (double?)b.Score) ?? 0;

            double score2025 = await _context.benchmarks
                .Where(b => b.HardwareId == hwid &&
                            b.TestPresetId == presetid &&
                            b.TestSubjectId == subjectid &&
                            b.Date == 2025)
                .AverageAsync(b => (double?)b.Score) ?? 0;

            if (score2022 == 0)
                return 0;

            return score2025 / score2022;
}

        // public async Task<List<int>> GetScore(int hwid)
        // {
        //     return [2,2];
        // }
        // Checking wheter the gpu has 2025 and 2022 benchmark
        public async Task<BenchmarkAvailability> GetBenchmarkAvailability(int hwid, int presetid, int subjectid)
        {
            bool has2022 = await _context.benchmarks
                .AnyAsync(b => b.HardwareId == hwid && 
                            b.TestPresetId == presetid && 
                            b.TestSubjectId == subjectid && 
                            b.Date == 2022);
            
            bool has2025 = await _context.benchmarks
                .AnyAsync(b => b.HardwareId == hwid && 
                            b.TestPresetId == presetid && 
                            b.TestSubjectId == subjectid && 
                            b.Date == 2025);
            
            if (has2022 && has2025) return BenchmarkAvailability.Both;
            if (has2022 && !has2025) return BenchmarkAvailability.Only2022;
            if (!has2022 && has2025) return BenchmarkAvailability.Only2025;
            return BenchmarkAvailability.None;
        }
        public enum BenchmarkAvailability
        {
            None,
            Only2022,
            Only2025,
            Both
        }
    }
}