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
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Index()
        {
            // Fetch the list of users
            var users = await _userManager.Users.ToListAsync();

            // Fetch grades with related data
            var grades = await _context.Grade
                .Include(g => g.Subject) // Include Subject details
                .ToListAsync();

            // Map StudentId to UserName dynamically
            foreach (var grade in grades)
            {
                var user = users.FirstOrDefault(u => u.Id == grade.StudentId);
                grade.StudentId = user?.StudentId ?? "Unknown"; // Dynamically set the student's name
            }

            return View(grades);
        }

        // GET: Grades/Details/5
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
        public async Task<IActionResult> Create()
        {
            // Lista e numrave dhe statuset e notave
            var numbers = new List<int> { 5, 6, 7, 8, 9, 10 };
            var gradeStatuses = new List<string> { "Normal", "Transfer" };

            // Merrni të gjithë përdoruesit me rolin "Student"
            var allUsers = await _userManager.Users.ToListAsync();
            var studentUsers = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Student"))
                {
                    studentUsers.Add(user);
                }
            }

            // Fetch the logged-in user's grades
            var userId = _userManager.GetUserId(User);
            var gradedSubjects = _context.Grade
                .Where(g => g.StudentId == userId)
                .Select(g => g.SubjectId)
                .ToHashSet();

            // Filter subjects that have not been graded yet
            var unGradedSubjects = _context.Subject
                .Where(s => !gradedSubjects.Contains(s.Id))
                .ToList();

            // Populoni ViewData me të dhënat e filtruara
            ViewData["Numbers"] = new SelectList(numbers);
            ViewData["GradeStatuses"] = new SelectList(gradeStatuses);
            ViewData["Users"] = new SelectList(studentUsers, "Id", "StudentId");
            ViewData["SubjectId"] = new SelectList(unGradedSubjects, "Id", "Name");

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

            if (ModelState.IsValid)
            {
                _context.Add(grade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
