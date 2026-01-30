using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using pcp2p.Models;

namespace pcp2p.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly AppDbContext _context;

        private bool Locked = false;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<AccountController> logger, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            var username = User.Identity.Name;
            if (username != null)
            {
                return RedirectToAction("Index", "Admin");
            }
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid login attempt");  
                return View(login);
            }
            var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, login.Rememberme, lockoutOnFailure: false);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Account locked. Please try again later.");
                return View(login);
            }
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Admin"); 
            }
            ModelState.AddModelError(string.Empty, "Wrong password or Username");
            return View(login);
        }
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity.Name;
            if (username == null)
            {
                _logger.LogInformation($"No user is logged in");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                _logger.LogInformation($"Logged out: {username}");
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
        }
    }
}