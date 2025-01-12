using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AuthSystem.Data;
using AuthSystem.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// Get connection string and configure services
var connectionString = builder.Configuration.GetConnectionString("AuthDbContextConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'AuthDbContextConnection' not found.");
}

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>() // Enable roles
.AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddRazorPages();

// Build the app
var app = builder.Build();

// Seed roles after building the app
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await SeedRolesAndAdminAsync(serviceProvider);
}

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

// Method to seed roles
async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var roles = new[] { "Admin", "Staff", "Student" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var adminEmail = "admin@admin.com";
    var adminPassword = "Kosova1234.";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var newAdmin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true, // Opsional
            FirstName = "Admin",
            LastName = "Administrator",
            PersonalNumber = "123456789",
            ParentName = "Parent Name",
            BirthDate = new DateTime(1990, 1, 1),
            Gender = "Male",
            BirthPlace = "Prishtina",
            State = "Kosova",
            Residence = "Prishtina",
            Address = "Rruga Admin",
            ZipCode = "10000",
            PrivateEmail = "private@admin.com",
            Nationality = "Kosovar",
            Citizenship = "Kosovar",
            DepartmentId = null 
        };

        var result = await userManager.CreateAsync(newAdmin, adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
        else
        {
            throw new Exception($"Nuk u krijua admini: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}

