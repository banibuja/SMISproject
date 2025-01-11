using Microsoft.AspNetCore.Identity;

namespace AuthSystem.Areas.Identity.Data
{
    public class SeedRoles
    {
        public static void Seed(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Student", "Staff" };

            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role).Result)
                {
                    var roleResult = roleManager.CreateAsync(new IdentityRole(role)).Result;
                }
            }
        }
    }
}
