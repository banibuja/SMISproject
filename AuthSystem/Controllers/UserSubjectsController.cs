using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuthSystem.Data;
using AuthSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace AuthSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserSubjectsController : Controller
    {
        private readonly AuthDbContext _context;

        public UserSubjectsController(AuthDbContext context)
        {
            _context = context;
        }

        // GET: UserSubjects
        public async Task<IActionResult> Index()
        {
            // Fetch subjects with at least one staff assigned
            var subjectStaffList = await _context.Subject
                .Where(s => s.UserSubjects.Any()) // Only subjects with staff assigned
                .Include(s => s.UserSubjects)    // Include UserSubjects (join table)
                .ThenInclude(us => us.User)      // Include User (staff) for each subject
                .ToListAsync();

            return View(subjectStaffList); // Returning the list of subjects to the view
        }






        // GET: UserSubjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userSubject = await _context.UserSubject
                .Include(u => u.Subject)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userSubject == null)
            {
                return NotFound();
            }

            return View(userSubject);
        }

        // GET: UserSubjects/Create
        public async Task<IActionResult> Create()
        {
            // Fetch all subjects
            var subjects = await _context.Subject.ToListAsync();

            // Fetch all staff with "Staff" role
            var staffUsers = await _context.Users
                .Where(user => _context.UserRoles
                    .Any(ur => ur.UserId == user.Id && _context.Roles
                        .Any(r => r.Id == ur.RoleId && r.Name == "Staff")))
                .ToListAsync();

            // Fetch UserSubjects to exclude already assigned staff for each subject
            var userSubjects = await _context.UserSubject.ToListAsync();

            // Pass all necessary data through ViewData
            ViewData["Subjects"] = subjects;
            ViewData["StaffUsers"] = staffUsers;
            ViewData["UserSubjects"] = userSubjects;

            return View();
        }




        // POST: UserSubjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,SubjectId")] UserSubject userSubject)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userSubject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Category", userSubject.SubjectId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userSubject.UserId);
            return View(userSubject);
        }

        // GET: UserSubjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userSubject = await _context.UserSubject.FindAsync(id);
            if (userSubject == null)
            {
                return NotFound();
            }
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Category", userSubject.SubjectId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userSubject.UserId);
            return View(userSubject);
        }

        // POST: UserSubjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,SubjectId")] UserSubject userSubject)
        {
            if (id != userSubject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userSubject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserSubjectExists(userSubject.Id))
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
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Category", userSubject.SubjectId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userSubject.UserId);
            return View(userSubject);
        }

        // GET: UserSubjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userSubject = await _context.UserSubject
                .Include(u => u.Subject)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userSubject == null)
            {
                return NotFound();
            }

            return View(userSubject);
        }

        // POST: UserSubjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userSubject = await _context.UserSubject.FindAsync(id);
            if (userSubject != null)
            {
                _context.UserSubject.Remove(userSubject);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserSubjectExists(int id)
        {
            return _context.UserSubject.Any(e => e.Id == id);
        }
    }
}
