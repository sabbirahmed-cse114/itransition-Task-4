using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskFour.Data;
using TaskFour.Models;

namespace TaskFour.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(model);
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Designation = model.Designation,
                    Email = model.Email,
                    LastLoginTime = DateTime.Now,
                    LastActivityTime = DateTime.Now,
                    IsBlocked = false
                };

                user.Password = _passwordHasher.HashPassword(user, model.Password);
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Account created successfully. Please log in.";
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid credentials.");
                    return View(model);
                }

                var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    if (user.IsBlocked)
                    {
                        ModelState.AddModelError("", " Your account is blocked.");
                        return View(model);
                    }
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("FullName", user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    user.LastLoginTime = DateTime.Now;
                    user.LastActivityTime = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "User");
                }
                ModelState.AddModelError("", "Invalid email or password.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var userEmail = User?.Identity?.Name;

            if (!string.IsNullOrEmpty(userEmail))
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user != null)
                {
                    user.LastActivityTime = DateTime.Now.AddSeconds(-02);
                    _context.SaveChanges();
                }
            }
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckEmail([FromBody] EmailCheckerModel model)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == model.Email);
            return Json(new { exists });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                return Json(new { success = false });
            }

            user.Password = _passwordHasher.HashPassword(user, model.NewPassword);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}