using AuthSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthSystem.Areas.Identity.Pages.Account
{

    [Authorize(Roles = "Admin")]

    public class AllStaffModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AllStaffModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IList<ApplicationUser> StaffUsers { get; set; }
        public Dictionary<ApplicationUser, IList<string>> UserRoles { get; set; }

        public async Task OnGetAsync()
        {
            // Merr të gjithë përdoruesit
            var allUsers = await _userManager.Users.ToListAsync();

            // Filtron përdoruesit me rolin "Staff"
            StaffUsers = new List<ApplicationUser>();
            UserRoles = new Dictionary<ApplicationUser, IList<string>>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Staff"))
                {
                    StaffUsers.Add(user);
                    UserRoles.Add(user, roles.ToList());
                }
            }
        }

        // Post handler për të fshirë një përdorues
        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToPage();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    // Redirect pas fshirjes
                    return RedirectToPage();
                }
            }

            ModelState.AddModelError(string.Empty, "Failed to delete the user.");
            return RedirectToPage();
        }
    }
}
