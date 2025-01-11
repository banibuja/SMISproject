using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthSystem.Areas.Identity.Data;
using AuthSystem.Data;

namespace AuthSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly AuthDbContext _context;

        public UserController(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Merr të gjithë përdoruesit
            var users = await _context.Users.ToListAsync();

            return View(users); // Sigurohu që kthehet view 'UserIndex'
        }


    }
}
