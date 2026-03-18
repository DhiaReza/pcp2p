using System.Xml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pcp2p.Models;

namespace pcp2p.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AdminController> _logger;
        private readonly AppDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<AdminController> logger, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var Cpu = await _context.Hardwares.Include(h => h.Cpu).Include(h => h.Brand).Where(h => h.Gpu != null).ToListAsync();
            var Gpu = await _context.Hardwares.Include(h => h.Gpu).Include(h => h.Brand).Where(h => h.Cpu != null).ToListAsync();
            var Brand = await _context.brands.ToListAsync();

            AdminPageDTO adminDTO = new AdminPageDTO
            {
                Cpus = Cpu,
                Gpus = Gpu,
                Brands = Brand,
            };

            return View(adminDTO);
        }

        public async Task<IActionResult> AddBrand(BrandDTO BrandInput)
        {
            if(BrandInput == null || !ModelState.IsValid)
            {
                return View(BrandInput);
            }
            var brand = new Brand
            {
                Name = BrandInput.Name
            };

            _context.Add(brand);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Success");

            return RedirectToAction("Index", "Admin");
        }

        public async Task<IActionResult> AddHardware()
        {
        var viewModel = new AddHardwareViewDTO
        {
            HardwareData = new AddHardwareDTO(), // Empty form
            BrandOptions = await _context.brands
                .ToListAsync(),
            TypeOptions = await _context.hardwareTypes
                .ToListAsync()
        };
        return View(viewModel);
        }
        
        // [HttpPost]
        // public async Task<IActionResult> AddHardware(AddHardwareViewDTO dto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         var viewModel = new AddHardwareViewDTO
        //         {
        //             HardwareData = new AddHardwareDTO(), // Empty form
        //             BrandOptions = await _context.brands
        //                 .ToListAsync(),
        //             TypeOptions = await _context.hardwareTypes
        //                 .ToListAsync()
        //         };
        //         return View(viewModel);
        //     }
            
        //     // 1. Create the base Hardware entity
        //     var hardware = new Hardware
        //     {
        //         Name = dto.HardwareData.Name,
        //         Generation = dto.HardwareData.Generation,
        //         MSRP = dto.HardwareData.MSRP,
        //         ReleaseDate = dto.HardwareData.ReleaseDate,
        //         BrandId = dto.HardwareData.BrandId,
        //         HardwareTypeId = dto.HardwareData.HardwareTypeId
        //     };

        //     // 2. Attach the specific sub-type data
        //     // Assuming 1 = CPU and 2 = GPU in your database
        //     switch (dto.HardwareData.HardwareTypeId)
        //     {
        //         case 1:
        //             hardware.Cpu = new Cpu
        //             {
        //                 CoreCount = dto.HardwareData.CoreCount ?? 0,
        //                 ThreadCount = dto.HardwareData.ThreadCount ?? 0,
        //                 BaseClock = dto.HardwareData.CpuBaseClock ?? 0,
        //                 TDP = dto.HardwareData.TDP ?? 0
        //             };
        //             break;
        //         case 2:
        //             hardware.Gpu = new Gpu
        //             {
        //                 Vram = dto.HardwareData.Vram ?? 0,
        //                 BaseClock = dto.HardwareData.GpuBaseClock ?? 0,
        //                 BoostClock = dto.HardwareData.BoostClock ?? 0,
        //                 GameClock = dto.HardwareData.GameClock ?? 0
        //             };
        //             break;
        //         default:
        //             ModelState.AddModelError("HardwareTypeId", "The selected hardware type is not currently supported.");
        //             return View(dto);
        // //     }

        //     _context.Hardwares.Add(hardware);
        //     await _context.SaveChangesAsync(); 
            
        //     return RedirectToAction("Index", "Admin");
        // }
        
    }
}