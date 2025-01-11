using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AuthSystem.Areas.Identity.Data;

namespace AuthSystem.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> MyProfile()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(currentUser);
        }
    }
}
