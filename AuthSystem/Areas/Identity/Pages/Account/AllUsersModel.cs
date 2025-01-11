using AuthSystem.Areas.Identity.Data;
using AuthSystem.Models;
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

    public class AllUsersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AllUsersModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IList<ApplicationUser> AllUsers { get; set; }
        public Dictionary<ApplicationUser, IList<string>> UserRoles { get; set; }

        public async Task OnGetAsync()
        {
            // Merr përdoruesit dhe përfshij të dhënat e departamentit
            var allUsers = await _userManager.Users
                .Include(u => u.Department) // Përfshij të dhënat e Department
                .ToListAsync();

            AllUsers = new List<ApplicationUser>();
            UserRoles = new Dictionary<ApplicationUser, IList<string>>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Student"))
                {
                    AllUsers.Add(user);
                    UserRoles.Add(user, roles.ToList());
                }
            }
        }
        // Post handler for deleting a user
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
                    // Redirect back to the same page after deleting
                    return RedirectToPage();
                }
            }

            // If there was an error, stay on the page and show a failure message
            ModelState.AddModelError(string.Empty, "Failed to delete the user.");
            return RedirectToPage();
        }
    }
}
