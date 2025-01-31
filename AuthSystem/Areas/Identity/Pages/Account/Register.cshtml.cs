﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using AuthSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using AuthSystem.Data;
using AuthSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthSystem.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly AuthDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            AuthDbContext context,
            RoleManager<IdentityRole> roleManager
        )
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public IList<Department> Departments { get; set; }
        public IList<string> Roles { get; set; }
        public IList<ApplicationUser> AllUsers { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Personal Number")]
            public string PersonalNumber { get; set; }

            [Display(Name = "Parent Name")]
            public string ParentName { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Birth Date")]
            public DateTime BirthDate { get; set; }

            [Required]
            [Display(Name = "Gender")]
            public string Gender { get; set; }

            [Display(Name = "Birth Place")]
            public string BirthPlace { get; set; }

            [Display(Name = "State")]
            public string State { get; set; }

            [Display(Name = "Residence")]
            public string Residence { get; set; }

            [Display(Name = "Address")]
            public string Address { get; set; }

            [Display(Name = "Zip Code")]
            public string ZipCode { get; set; }

            [Display(Name = "Private Email")]
            public string PrivateEmail { get; set; }

            [Display(Name = "Nationality")]
            public string Nationality { get; set; }

            [Display(Name = "Citizenship")]
            public string Citizenship { get; set; }

            [Required]
            [Display(Name = "Department")]
            public int DepartmentId { get; set; }

            [Required]
            [Display(Name = "Role")]
            public string SelectedRole { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            Departments = await _context.Department.ToListAsync();
            AllUsers = await _userManager.Users.ToListAsync();

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.PersonalNumber = Input.PersonalNumber;
                user.ParentName = Input.ParentName;
                user.BirthDate = Input.BirthDate;
                user.Gender = Input.Gender;
                user.BirthPlace = Input.BirthPlace;
                user.State = Input.State;
                user.Residence = Input.Residence;
                user.Address = Input.Address;
                user.ZipCode = Input.ZipCode;
                user.PrivateEmail = Input.PrivateEmail;
                user.Nationality = Input.Nationality;
                user.Citizenship = Input.Citizenship;
                user.DepartmentId = Input.DepartmentId;

                // Generate email based on role
                string generatedEmail;
                if (Input.SelectedRole == "Admin" || Input.SelectedRole == "Staff")
                {
                    generatedEmail = GenerateUniqueEmail(user.FirstName, user.LastName); // Ensure uniqueness for Admin/Staff
                }
                else
                {
                    // Generate email for other roles (e.g., Student)
                    string studentId = GenerateStudentId();
                    generatedEmail = GenerateEmail(user.FirstName, user.LastName, studentId);
                    user.StudentId = studentId;
                }
                user.Email = generatedEmail;

                // Set the username and email
                await _userStore.SetUserNameAsync(user, user.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, user.Email, CancellationToken.None);

                string randomPassword = GenerateRandomPassword();

                // Create user in the database
                var result = await _userManager.CreateAsync(user, randomPassword);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, Input.SelectedRole);
                    await SendRegistrationDetailsEmail(user, randomPassword);

                    var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    string logAction = Input.SelectedRole switch
                    {
                        "Admin" => "Admin Created",
                        "Staff" => "Staff Created",
                        "Student" => "Student Created",
                        _ => "User Created"
                    };

                    var log = new Log
                    {
                        Action = logAction,
                        UserId = loggedInUserId,
                        Timestamp = DateTime.Now,
                        Details = $"created with email {user.Email}."
                    };
                    _context.Logs.Add(log);  // Assuming you have a DbSet<Log> in your context
                    await _context.SaveChangesAsync();

                    return LocalRedirect(returnUrl);
                }
            

            foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private string GenerateUniqueEmail(string firstName, string lastName)
        {
            string baseEmail = $"{firstName.Substring(0, 1).ToLower()}{lastName.ToLower()}@smis-uni.net";
            int count = _userManager.Users.Count(u => u.Email.StartsWith(baseEmail));
            return count > 0 ? $"{firstName.Substring(0, 1).ToLower()}{lastName.ToLower()}{count}@smis-uni.net" : baseEmail;
        }

        private string GenerateRandomPassword()
        {
            var options = new PasswordOptions
            {
                RequiredLength = 8,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonAlphanumeric = true
            };

            string[] randomChars = {
        "ABCDEFGHJKLMNOPQRSTUVWXYZ", // Uppercase
        "abcdefghijkmnopqrstuvwxyz", // Lowercase
        "0123456789",                // Digits
        "!@$?_-"                     // Special Characters
    };

            Random rand = new Random(Environment.TickCount);
            var chars = new List<char>();

            if (options.RequireUppercase)
                chars.Add(randomChars[0][rand.Next(randomChars[0].Length)]);

            if (options.RequireLowercase)
                chars.Add(randomChars[1][rand.Next(randomChars[1].Length)]);

            if (options.RequireDigit)
                chars.Add(randomChars[2][rand.Next(randomChars[2].Length)]);

            if (options.RequireNonAlphanumeric)
                chars.Add(randomChars[3][rand.Next(randomChars[3].Length)]);

            while (chars.Count < options.RequiredLength)
            {
                string rcs = randomChars[rand.Next(randomChars.Length)];
                chars.Add(rcs[rand.Next(rcs.Length)]);
            }

            return new string(chars.OrderBy(x => rand.Next()).ToArray());
        }

        private string GenerateEmail(string firstName, string lastName, string studentId)
        {
            string firstCharacterFirstName = firstName.Substring(0, 1).ToLower();
            string firstCharacterLastName = lastName.Substring(0, 1).ToLower();
            return $"{firstCharacterFirstName}{firstCharacterLastName}{studentId}@smis-uni.net";
        }

        private string GenerateStudentId()
        {
            var currentYear = DateTime.Now.Year.ToString().Substring(2);
            var nextYear = (DateTime.Now.Year + 1).ToString().Substring(2);
            var randomNumber = new Random().Next(1000, 9999);
            return $"{currentYear}{nextYear}{randomNumber}";
        }

        private async Task SendRegistrationDetailsEmail(ApplicationUser user, string randomPassword)
        {
            var emailSettings = new
            {
                SMTPServer = "smtp.gmail.com",
                SMTPPort = 587,
                SMTPUsername = "shaban.buja111@gmail.com",
                SMTPPassword = "ypjw vknu wqnj xwlg",
                SMTPFromEmail = "shaban.buja111@gmail.com"
            };

            try
            {
                using (var client = new System.Net.Mail.SmtpClient(emailSettings.SMTPServer, emailSettings.SMTPPort))
                {
                    client.Credentials = new System.Net.NetworkCredential(emailSettings.SMTPUsername, emailSettings.SMTPPassword);
                    client.EnableSsl = true;

                    var mailMessage = new System.Net.Mail.MailMessage
                    {
                        From = new System.Net.Mail.MailAddress(emailSettings.SMTPFromEmail),
                        Subject = "Regjistrimi juaj është kryer me sukses",
                        Body = $@"
                    Përshëndetje {user.FirstName} {user.LastName},

                    Ju falënderojmë që u regjistruat në platformën tonë.
                    Këtu janë të dhënat tuaja të regjistrimit:
                    
                    Email: {user.Email}
                    Password: {randomPassword} (ruajeni këtë fjalëkalim të sigurt)                
                    
                    Ju lutem mos e shpërndani këtë informacion.

                    Me respekt,
                    Ekipi i mbështetjes",
                        IsBodyHtml = false
                    };

                    mailMessage.To.Add(user.PrivateEmail);

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation("Email sent successfully to {0}", user.PrivateEmail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send email: {0}", ex.Message);
            }
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
