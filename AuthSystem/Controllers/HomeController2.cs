using AuthSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthSystem.Controllers
{

    public class RoleController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
            {
                return Ok($"Role '{role}' assigned to user '{user.UserName}'.");
            }

            return BadRequest(result.Errors);
        }
    }
}
