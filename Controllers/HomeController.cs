using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using pcp2p.Models;

namespace pcp2p.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Cpu()
        {
            return View();
        }
        public IActionResult Gpu()
        {
            return View();
        }
        public IActionResult Compare()
        {
            return View();
        }        
    }
}
