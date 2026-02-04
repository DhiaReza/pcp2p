using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult AddHardware()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddHardware(HardwareCreateDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            // 1. Create the base Hardware entity
            var hardware = new Hardware
            {
                Name = dto.Name,
                Generation = dto.Generation,
                MSRP = dto.MSRP,
                ReleaseDate = dto.ReleaseDate,
                BrandId = dto.BrandId,
                HardwareTypeId = dto.HardwareTypeId
            };

            // 2. Attach the specific sub-type data
            // Assuming 1 = CPU and 2 = GPU in your database
            if (dto.HardwareTypeId == 1) 
            {
                hardware.Cpu = new Cpu {
                    CoreCount = dto.CoreCount ?? 0,
                    ThreadCount = dto.ThreadCount ?? 0,
                    BaseClock = dto.CpuBaseClock ?? 0,
                    // ... map other fields
                };
            }
            else if (dto.HardwareTypeId == 2)
            {
                hardware.Gpu = new Gpu {
                    Vram = dto.Vram ?? 0,
                    BaseClock = dto.GpuBaseClock ?? 0,
                    // ... map other fields
                };
            }

            _context.Hardwares.Add(hardware);
            await _context.SaveChangesAsync(); 
            
            return RedirectToAction("Index", "Admin");
        }
    }
}