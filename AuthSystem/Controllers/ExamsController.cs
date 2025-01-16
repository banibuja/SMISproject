using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuthSystem.Data;
using AuthSystem.Models;
using Microsoft.AspNetCore.Identity;
using AuthSystem.Areas.Identity.Data;

namespace AuthSystem.Controllers
{
    public class ExamsController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExamsController(AuthDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: Exams
        public async Task<IActionResult> Index()
        {
            var authDbContext = _context.Exam
                .Include(e => e.UserSubject.User)
                .Include(e => e.UserSubject)
                .Include(e => e.UserSubject.Subject)
                .Select(e => new
                {
                    Id = e.Id,
                    userId = e.UserId,
                    SubjectCode = e.UserSubject.Subject.Code,
                    SubjectName = e.UserSubject.Subject.Name,
                    SubjectCategory = e.UserSubject.Subject.Category,
                    StaffName = e.UserSubject.User.UserName,
                    Grade = _context.Grade
                                .Where(g => g.StudentId == _userManager.GetUserId(User) && g.SubjectId == e.UserSubject.Subject.Id)
                                .Select(g => g.Number)
                                .FirstOrDefault(),
                    GradeStatus = _context.Grade
                                .Where(g => g.StudentId == _userManager.GetUserId(User) && g.SubjectId == e.UserSubject.Subject.Id)
                                .Select(g => g.GradeStatus)
                                .FirstOrDefault(),
                    CreatedAt = _context.Grade
                                .Where(g => g.StudentId == _userManager.GetUserId(User) && g.SubjectId == e.UserSubject.Subject.Id)
                                .Select(g => g.CreatedAt)
                                .FirstOrDefault()
                })
                .Where(e => e.userId == _userManager.GetUserId(User));

            return View(await authDbContext.ToListAsync());
        }


        // GET: Exams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exam = await _context.Exam
                .Include(e => e.User)
                .Include(e => e.UserSubject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        // GET: Exams/Create
        public IActionResult Create()
        {
            // Get the logged-in user's departmentId
            var userId = _userManager.GetUserId(User); // Get the logged-in user's ID
            var loggedInUserDepartmentId = _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.DepartmentId)
                .FirstOrDefault(); // Get the departmentId of the logged-in user

            // Fetch all subjects with assigned staff, excluding those already in Exam rows,
            // excluding subjects where the user already has a grade, and filtering by departmentId
            var userSubjects = _context.UserSubject
                .Include(us => us.Subject)  // Include Subject details
                .Include(us => us.User)     // Include User details
                .Where(us => !_context.Exam.Any(e => e.UserSubject.SubjectId == us.SubjectId) && // Exclude Exam rows
                             !_context.Grade.Any(g => g.StudentId == userId && g.SubjectId == us.Subject.Id) && // Exclude subjects with grades
                             us.Subject.DepartmentId == loggedInUserDepartmentId) // Filter by departmentId
                .GroupBy(us => us.SubjectId)
                .Select(group => new
                {
                    SubjectId = group.Key,
                    SubjectCode = group.First().Subject.Code,
                    SubjectName = group.First().Subject.Name,
                    SubjectCategory = group.First().Subject.Category,
                    AssignedStaff = group.Select(us => new
                    {
                        UserSubjectId = us.Id,
                        UserId = us.UserId,
                        UserName = us.User.UserName
                    }).ToList()
                })
                .ToList();

            return View(userSubjects);
        }











        // POST: Exams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserSubjectId")] Exam exam)
        {
            exam.UserId = _userManager.GetUserId(User);

            if (exam.UserSubjectId == null)
            {
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", exam.UserId);
                ViewData["UserSubjectId"] = new SelectList(_context.UserSubject, "Id", "Id", exam.UserSubjectId);
                return View(exam);
            }
            _context.Add(exam);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Exams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exam = await _context.Exam.FindAsync(id);
            if (exam == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", exam.UserId);
            ViewData["UserSubjectId"] = new SelectList(_context.UserSubject, "Id", "Id", exam.UserSubjectId);
            return View(exam);
        }

        // POST: Exams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserSubjectId,UserId")] Exam exam)
        {
            if (id != exam.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamExists(exam.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", exam.UserId);
            ViewData["UserSubjectId"] = new SelectList(_context.UserSubject, "Id", "Id", exam.UserSubjectId);
            return View(exam);
        }

        // GET: Exams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exam = await _context.Exam
                .Include(e => e.User)
                .Include(e => e.UserSubject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        // POST: Exams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exam = await _context.Exam.FindAsync(id);
            if (exam != null)
            {
                _context.Exam.Remove(exam);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamExists(int id)
        {
            return _context.Exam.Any(e => e.Id == id);
        }
    }
}
