using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace StudentRecordManager.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
public IActionResult Login()
{
    return View("~/Views/Login.cshtml");
}
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Simple mock authorization check (change "admin" and "password123" later!)
            if (username == "admin" && password == "password123")
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);
                return RedirectToAction("Index", "Students");
            }

           ViewBag.Error = "Invalid administrative login credentials.";
return View("~/Views/Login.cshtml");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Index", "Home");
        }
    }
}
