using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuthSystem.Data;
using AuthSystem.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AuthSystem.Controllers
{
    public class StudentSemestersController : Controller
    {
        private readonly AuthDbContext _context;

        public StudentSemestersController(AuthDbContext context)
        {
            _context = context;
        }

        // GET: StudentSemesters

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Index()
        {
            var authDbContext = _context.StudentSemester
                .Include(s => s.Department)
                .Include(s => s.Location)
                .Include(s => s.Schedule)
                .Include(s => s.Semester)
                .Include(s => s.User);
            return View(await authDbContext.ToListAsync());
        }

        // GET: StudentSemesters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentSemester = await _context.StudentSemester
                .Include(s => s.Department)
                .Include(s => s.Location)
                .Include(s => s.Schedule)
                .Include(s => s.Semester)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.StudentSemesterId == id);
            if (studentSemester == null)
            {
                return NotFound();
            }

            return View(studentSemester);
        }

        // GET: StudentSemesters/Create
        [Authorize(Roles = "Student")]
        public IActionResult Create()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the logged-in user's department
            var loggedInUserDepartmentId = _context.Users
                .Where(u => u.Id == loggedInUserId)
                .Select(u => u.DepartmentId)
                .FirstOrDefault();

            // Filter the departments to show only the user's department
            var userDepartment = _context.Department
                .Where(d => d.Id == loggedInUserDepartmentId)
                .ToList();

            // Populate other dropdowns
            ViewData["DepartmentId"] = new SelectList(userDepartment, "Id", "Name");
            ViewData["LocationId"] = new SelectList(_context.Location, "LocationId", "Name");
            ViewData["ScheduleId"] = new SelectList(_context.Schedule, "ScheduleId", "Name");

            // Get the list of registered semesters for the logged-in user
            var registeredSemesterIds = _context.StudentSemester
                .Where(ss => ss.UserId == loggedInUserId)
                .Select(ss => ss.SemesterId)
                .ToList();

            // Get the list of available semesters not registered by the user
            var availableSemesters = _context.Semester
                .Where(s => !registeredSemesterIds.Contains(s.SemesterId))
                .ToList();

            ViewData["SemesterId"] = new SelectList(availableSemesters, "SemesterId", "Name");

            // Filter users to include only the logged-in user
            var loggedInUser = _context.Users
                .Where(u => u.Id == loggedInUserId)
                .Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName })
                .ToList();

            ViewData["UserId"] = new SelectList(loggedInUser, "Id", "FullName");

            // Fetch the logged-in user's semester registrations
            var studentSemesters = _context.StudentSemester
                .Where(ss => ss.UserId == loggedInUserId)
                .Include(ss => ss.Semester)
                .Include(ss => ss.Location)
                .Include(ss => ss.Schedule)
                .Include(ss => ss.Department)
                .ToList();

            // Pass the studentSemesters to the view using ViewData
            ViewData["StudentSemesters"] = studentSemesters;

            return View();
        }



        // POST: StudentSemesters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentSemesterId,SemesterId,LocationId,ScheduleId,DepartmentId,UserId")] StudentSemester studentSemester)
        {
            if (ModelState.IsValid)
            {
                // Check if the student has already registered for the selected semester
                var existingRegistration = await _context.StudentSemester
                    .FirstOrDefaultAsync(ss => ss.UserId == studentSemester.UserId && ss.SemesterId == studentSemester.SemesterId);

                if (existingRegistration != null)
                {
                    // If a registration exists for this student and semester, show an error message
                    ModelState.AddModelError(string.Empty, "You have already registered for this semester.");

                    // Repopulate the ViewData for the dropdowns, but exclude the already selected semester
                    var availableSemesters = _context.Semester
                        .Where(s => s.SemesterId != studentSemester.SemesterId)
                        .ToList();

                    ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Name", studentSemester.DepartmentId);
                    ViewData["LocationId"] = new SelectList(_context.Location, "LocationId", "Name", studentSemester.LocationId);
                    ViewData["ScheduleId"] = new SelectList(_context.Schedule, "ScheduleId", "Name", studentSemester.ScheduleId);
                    ViewData["SemesterId"] = new SelectList(availableSemesters, "SemesterId", "Name", studentSemester.SemesterId);
                    ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", studentSemester.UserId);
                    return View(studentSemester);
                }

                // No existing registration, so add the new registration
                _context.Add(studentSemester);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }

            // If model state is not valid, repopulate dropdown lists for the view
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Name", studentSemester.DepartmentId);
            ViewData["LocationId"] = new SelectList(_context.Location, "LocationId", "Name", studentSemester.LocationId);
            ViewData["ScheduleId"] = new SelectList(_context.Schedule, "ScheduleId", "Name", studentSemester.ScheduleId);
            ViewData["SemesterId"] = new SelectList(_context.Semester, "SemesterId", "Name", studentSemester.SemesterId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", studentSemester.UserId);
            return View(studentSemester);
        }

      



        // GET: StudentSemesters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentSemester = await _context.StudentSemester.FindAsync(id);
            if (studentSemester == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Id", studentSemester.DepartmentId);
            ViewData["LocationId"] = new SelectList(_context.Location, "LocationId", "Name", studentSemester.LocationId);
            ViewData["ScheduleId"] = new SelectList(_context.Schedule, "ScheduleId", "Name", studentSemester.ScheduleId);
            ViewData["SemesterId"] = new SelectList(_context.Semester, "SemesterId", "Name", studentSemester.SemesterId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", studentSemester.UserId);
            return View(studentSemester);
        }

        // POST: StudentSemesters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentSemesterId,SemesterId,LocationId,ScheduleId,DepartmentId,UserId")] StudentSemester studentSemester)
        {
            if (id != studentSemester.StudentSemesterId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentSemester);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentSemesterExists(studentSemester.StudentSemesterId))
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
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Id", studentSemester.DepartmentId);
            ViewData["LocationId"] = new SelectList(_context.Location, "LocationId", "Name", studentSemester.LocationId);
            ViewData["ScheduleId"] = new SelectList(_context.Schedule, "ScheduleId", "Name", studentSemester.ScheduleId);
            ViewData["SemesterId"] = new SelectList(_context.Semester, "SemesterId", "Name", studentSemester.SemesterId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", studentSemester.UserId);
            return View(studentSemester);
        }

        // GET: StudentSemesters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentSemester = await _context.StudentSemester
                .Include(s => s.Department)
                .Include(s => s.Location)
                .Include(s => s.Schedule)
                .Include(s => s.Semester)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.StudentSemesterId == id);
            if (studentSemester == null)
            {
                return NotFound();
            }

            return View(studentSemester);
        }

        // POST: StudentSemesters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentSemester = await _context.StudentSemester.FindAsync(id);
            if (studentSemester != null)
            {
                _context.StudentSemester.Remove(studentSemester);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentSemesterExists(int id)
        {
            return _context.StudentSemester.Any(e => e.StudentSemesterId == id);
        }
    }
}
