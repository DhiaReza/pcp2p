using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pcp2p.Models;

namespace pcp2p.Controllers
{
    public class CatalogueController : Controller
    {
        private readonly ILogger<CatalogueController> _logger;
        private readonly AppDbContext _context;
        public int pageSize = 10;
        private bool Locked = false;
        public CatalogueController(ILogger<CatalogueController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public ActionResult Index(
            string searchString,
            string sortOrder,
            int currentPage = 1)
        {
            var result = GetCpus(
                searchString,
                sortOrder,  
                currentPage,
                pageSize);

            List<Hardware> cpus = result.Items;

            int totalCount = result.TotalCount;

            int maxPage = (int)Math.Ceiling((double)totalCount / pageSize);

            ViewBag.MaxPage = maxPage;

            ViewBag.CurrentPage = currentPage;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentSearchString = searchString;

            return View(cpus);
        }
        public (List<Hardware> Items, int TotalCount) GetCpus(
            string searchString,
            string sortOrder,
            int currentPage,
            int pageSize)
        {
            if(currentPage < 1)
                currentPage = 1;

            var cpus = _context.Hardwares
                .Include(h => h.Brand)
                .Include(h => h.Cpu)
                .Include(h => h.Gpu)

                .Include(h => h.Benchmarks.Where(b => b.TestSource.Name != "interpolated"))
                    .ThenInclude(b => b.TestSource)

                .Include(h => h.Benchmarks)
                    .ThenInclude(b => b.TestSubject)

                .Include(h => h.Benchmarks)
                    .ThenInclude(b => b.TestPreset)

                .AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                cpus = cpus.Where(h =>
                    h.Name.Contains(searchString.ToUpper()) ||
                    h.MSRP.ToString().Contains(searchString) ||
                    h.Brand.Name.Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "name_asc":
                    cpus = cpus.OrderBy(h => h.Name);
                    break;

                case "name_desc":
                    cpus = cpus.OrderByDescending(h => h.Name);
                    break;

                case "oldest":
                    cpus = cpus.OrderBy(h => h.ReleaseDate);
                    break;

                case "newest":
                    cpus = cpus.OrderByDescending(h => h.ReleaseDate);
                    break;

                case "cheapest":
                    cpus = cpus.OrderBy(h => h.MSRP);
                    break;

                case "expensive":
                    cpus = cpus.OrderByDescending(h => h.MSRP);
                    break;

                default:
                    cpus = cpus.OrderByDescending(h => h.ReleaseDate);
                    break;
            }

            int totalCount = cpus.Count();

            List<Hardware> hw = cpus
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (hw, totalCount);
        }
    }
}