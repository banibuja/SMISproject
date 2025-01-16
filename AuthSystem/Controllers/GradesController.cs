using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuthSystem.Data;
using AuthSystem.Models;
using AuthSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using AuthSystem.Migrations;
using System.Security.Claims;

namespace AuthSystem.Controllers
{
    public class GradesController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GradesController(AuthDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Grades
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Index()
        {
            // Fetch the list of users
            var users = await _userManager.Users.ToListAsync();
            var professorId = _userManager.GetUserId(User);

            // Fetch grades with related data
            var grades = await _context.Grade
                .Include(g => g.Subject)
                .Where(g => g.UserId == professorId)// Include Subject details
                .ToListAsync();

            // Map StudentId to UserName dynamically
            foreach (var grade in grades)
            {
                var user = users.FirstOrDefault(u => u.Id == grade.StudentId);
                grade.StudentId = user?.UserName ?? "Unknown"; // Dynamically set the student's name
            }

            return View(grades);
        }

        // GET: Grades/Details/5
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Details()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound();
            }

            // Fetch all grades for the logged-in student
            var grades = await _context.Grade
                .Include(g => g.Subject)
                .Where(m => m.StudentId == userId)
                .ToListAsync();

            if (grades == null || !grades.Any())
            {
                return NotFound();
            }

            // Return partial view with all grades
            return PartialView("_GradeDetails", grades);
        }

        // GET: Grades/Create
        // GET: Grades/Create
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Create()
        {
            var numbers = new List<int> { 5, 6, 7, 8, 9, 10 };
            var gradeStatuses = new List<string> { "Normal", "Transfer" };

            var professorId = _userManager.GetUserId(User);

            var usersWithSubmittedExams = _context.Exam
                .Include(e => e.User) 
                .Include(e => e.UserSubject) 
                .Where(e => e.UserSubject.UserId == professorId) 
                .Select(e => e.User) 
                .Distinct() 
                .ToList();

            var userId = _userManager.GetUserId(User);
            var gradedSubjects = _context.Grade
                .Where(g => g.StudentId == userId)
                .Select(g => g.SubjectId)
                .ToHashSet();

            var unGradedSubjects = _context.UserSubject
                .Include(us => us.Subject) 
                .Where(us => us.UserId == userId) 
                .Select(us => us.Subject) 
                .Distinct() 
                .ToList();


            // Populoni ViewData me të dhënat e filtruara
            ViewData["Numbers"] = new SelectList(numbers);
            ViewData["GradeStatuses"] = new SelectList(gradeStatuses);
            ViewData["Users"] = new SelectList(usersWithSubmittedExams, "Id", "UserName");
            ViewData["SubjectId"] = new SelectList(unGradedSubjects, "Id", "Name");
            ViewData["UserId"] = new SelectList( new List<string> { userId });
            return View();
        }

        // POST: Grades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,Letter,GradeStatus,StudentId,SubjectId")] Grade grade)
        {
            // Check if a grade for the same subject and student already exists
            var existingGrade = await _context.Grade
                .FirstOrDefaultAsync(g => g.StudentId == grade.StudentId && g.SubjectId == grade.SubjectId);

            if (existingGrade != null)
            {
                ModelState.AddModelError("", "This student has already been graded for this subject.");
            }

            // Add UserId from the logged-in user
            grade.UserId = _userManager.GetUserId(User); // Assuming you're using ASP.NET Identity

            // Manually mark the UserId field as valid
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                _context.Add(grade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Populate dropdown for the view
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name", grade.SubjectId);
            return View(grade);
        }



        // GET: Grades/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grade = await _context.Grade.FindAsync(id);
            if (grade == null)
            {
                return NotFound();
            }
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Category", grade.SubjectId);
            return View(grade);
        }

        // POST: Grades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Letter,GradeStatus,StudentId,SubjectId")] Grade grade)
        {
            if (id != grade.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GradeExists(grade.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Category", grade.SubjectId);
            return View(grade);
        }

        // GET: Grades/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grade = await _context.Grade
                .Include(g => g.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (grade == null)
            {
                return NotFound();
            }

            return View(grade);
        }

        // POST: Grades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grade = await _context.Grade.FindAsync(id);
            if (grade != null)
            {
                _context.Grade.Remove(grade);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GradeExists(int id)
        {
            return _context.Grade.Any(e => e.Id == id);
        }
    }
}
