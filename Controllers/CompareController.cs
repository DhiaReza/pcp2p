using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Cpu()
        {
            
            return View("SelectHardware");
        }
        public async Task<IActionResult> Gpu()  
        {
            var data = new SelectHardwareDTO{
                // sent hardware selection
                hardwares =  _context.Hardwares
                .Where(b => b.HardwareTypeId == 1)
                .Select(b => new HardwareOptions { 
                    Id = b.Id, 
                    Name = b.Name, 
                    Vram = b.Gpu.Vram,
                    MSRP = b.MSRP,
                    Generation = b.Gpu.Generation }).ToList(),

                // sent subjects eg rt and raster
                testSubjects = _context.testSubjects
                .Where(b => b.Name == "raster" || b.Name == "raytracing")
                .ToList(),

                testPresets = _context.testPresets.ToList()
            };
            return View("SelectHardware", data);
        }
        public async Task<IActionResult> HardwareComparison(List<int> hardwareids, int presetid, int subjectid)
        {
            return View();
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