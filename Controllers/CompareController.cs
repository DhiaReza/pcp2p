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
        public IActionResult HardwareComparison(
            List<int>? hardwareids,
            int? presetid,
            int? subjectid,
            int hwtypeid)
        {
            if (hardwareids == null || !hardwareids.Any())
            {
                ModelState.AddModelError("hardwares", "Please select at least one hardware!");
            }

            if (presetid == null || presetid <= 0)
            {
                ModelState.AddModelError("testPresets", "Please select a valid preset!");
            }

            if (subjectid == null || subjectid <= 0)
            {
                ModelState.AddModelError("testSubjects", "Please select a valid category!");
            }

            if (!ModelState.IsValid)
            {
                hw = GetHardwareSelection(hwtypeid);

                return View("SelectHardware", hw);
            }

            // comparison query logic here

            return View();
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
        // get anchor factor score GPU(s) that has both 2025 and 2022 benchmark
        // divide 2025 benchmark score with 2022 benchmark score
        // do this with several gpus to get a precise factor score

        // get anchor
        public async Task<double> GetAnchorFactor()
        {
            // anchor gpus RTX 4060, RX 7600, 4090, 7800 XT, 4070, 7700.
            return 1.0;
        }

        // Checking wheter the gpu has 2025 and 2022 benchmark or not then return true if yes
        public async Task<bool> CheckBenchmarkDate(int hwid)
        {
            var hw2022 = _context.benchmarks.AnyAsync(b => b.Id == hwid && b.Date == 2022);
            var hw2025 = _context.benchmarks.AnyAsync(b => b.Id == hwid && b.Date == 2025);
            if (hw2022 == hw2025)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}