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

        private bool Locked = false;
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
        public IActionResult AddHardware()
        {
            return View();
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
    }
}