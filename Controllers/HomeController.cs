using System.Diagnostics;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pcp2p.Models;

namespace pcp2p.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        private bool Locked = false;
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Catalogue(
            string? searchString,
            string? sortOrder = null, 
            List<string>? filterGeneration= null
            )
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.NameSort = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.DateSort = sortOrder == "newest" ? "oldest" : "newest";
            ViewBag.PriceSort = sortOrder == "expensive" ? "cheap" : "expensive";
            
            List<Hardware> cpus = GetCpus(searchString,sortOrder, filterGeneration);
            return View(cpus);
        }
        public IActionResult Gpu()
        {
            return View();
        }
        public IActionResult Compare()
        {
            return View();
        }
        public IActionResult Detail(string? hardware)
        {
            return View();
        }

        public List<Hardware> GetCpus(
            string? searchString = null, 
            string? sortOrder = null, 
            List<string>? filterGeneration= null)
        {
            List<Hardware> cpus = null;

            cpus = _context.Hardwares
                .Include(h => h.Brand)
                .Include(h => h.Cpu)
                .Include(h => h.Gpu)
                .Include(h => h.Benchmarks.Where(b => b.TestSource.Name != "interpolated"))
                    .ThenInclude(b => b.TestSource)
                .Include(h => h.Benchmarks)
                    .ThenInclude(b => b.TestSubject)
                .Include(h => h.Benchmarks)
                    .ThenInclude(b => b.TestResolution)
                .Include(h => h.Benchmarks)
                    .ThenInclude(b => b.TestGraphic)
                    .ToList();
            
            if (!String.IsNullOrEmpty(searchString))
            {
                cpus = cpus.Where(h => 
                    h.Name.ToUpper().Contains(searchString.ToUpper())|| 
                    h.MSRP.ToString().Contains(searchString) || 
                    h.Brand.Name.ToUpper().Contains(searchString.ToUpper()))
                    .ToList();
            }
            switch (sortOrder)
            {
                case "name_asc":
                    cpus = cpus.OrderBy(h => h.Name).ToList();
                    break;
                case "name_desc":
                    cpus = cpus.OrderByDescending(h => h.Name).ToList();
                    break;
                case "oldest":
                    cpus = cpus.OrderBy(h => h.ReleaseDate).ToList();
                    break;
                case "newest":
                    cpus = cpus.OrderByDescending(h => h.ReleaseDate).ToList();
                    break;
                case "cheapest":
                    cpus = cpus.OrderBy(h => h.MSRP).ToList();
                    break;
                case "expensive":
                    cpus = cpus.OrderByDescending(h => h.MSRP).ToList();
                    break;
                //case ""
                default:
                    cpus = cpus.OrderByDescending(h => h.ReleaseDate).ToList();
                    break;
            }

            return cpus;
        }
    }
}
