using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentRecordManager.Models;

namespace StudentRecordManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            if (await _context.UserAccounts.AnyAsync(u => u.Username == username))
            {
                ViewBag.Error = "Username is already taken by an administrator account.";
                return View("~/Views/Register.cshtml");
            }

            var newUser = new UserAccount
            {
                Username = username,
                PasswordHash = password // In production apps, hash this using BCrypt!
            };

            _context.UserAccounts.Add(newUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Query database rows directly to find matching system credentials
            var user = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password);

            if (user != null)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Username) };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);
                return RedirectToAction("Index", "Students");
            }

            ViewBag.Error = "Invalid credential validation checks matched.";
            return View("~/Views/Login.cshtml");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Index", "Home");
        }
    }
}
