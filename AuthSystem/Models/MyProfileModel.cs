using AuthSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace AuthSystem.Models
{
    public class MyProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public MyProfileModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUser CurrentUser { get; set; }

        public async Task OnGetAsync()
        {
            // Merr ID e përdoruesit të loguar
            var userId = _userManager.GetUserId(User);

            // Merr të dhënat e përdoruesit nga databaza
            CurrentUser = await _userManager.FindByIdAsync(userId);
        }
    }
}