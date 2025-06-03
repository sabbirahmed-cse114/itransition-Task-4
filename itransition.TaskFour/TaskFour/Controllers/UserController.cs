using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFour.Data;

namespace TaskFour.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity?.Name;

            if (!string.IsNullOrEmpty(userEmail))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (user != null)
                {
                    user.LastActivityTime = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
            }

            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block([FromBody] List<Guid> userIds)
        {
            foreach (var id in userIds)
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    user.IsBlocked = true;
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unblock([FromBody] List<Guid> userIds)
        {
            foreach (var id in userIds)
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    user.IsBlocked = false;
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] List<Guid> userIds)
        {
            foreach (var id in userIds)
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
