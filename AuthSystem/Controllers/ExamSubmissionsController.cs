using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuthSystem.Data;
using AuthSystem.Models;

namespace AuthSystem.Controllers
{
    public class ExamSubmissionsController : Controller
    {
        private readonly AuthDbContext _context;

        public ExamSubmissionsController(AuthDbContext context)
        {
            _context = context;
        }

        // GET: ExamSubmissions
        public async Task<IActionResult> Index()
        {
            var authDbContext = _context.ExamSubmission.Include(e => e.Department);
            return View(await authDbContext.ToListAsync());
        }

        // GET: ExamSubmissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examSubmission = await _context.ExamSubmission
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.ExamSubmissionId == id);
            if (examSubmission == null)
            {
                return NotFound();
            }

            return View(examSubmission);
        }

        // GET: ExamSubmissions/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Id");
            return View();
        }

        // POST: ExamSubmissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExamSubmissionId,ExamTitle,SubmissionDate,Status,DepartmentId")] ExamSubmission examSubmission)
        {
            if (ModelState.IsValid)
            {
                _context.Add(examSubmission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Id", examSubmission.DepartmentId);
            return View(examSubmission);
        }

        // GET: ExamSubmissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examSubmission = await _context.ExamSubmission.FindAsync(id);
            if (examSubmission == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Id", examSubmission.DepartmentId);
            return View(examSubmission);
        }

        // POST: ExamSubmissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExamSubmissionId,ExamTitle,SubmissionDate,Status,DepartmentId")] ExamSubmission examSubmission)
        {
            if (id != examSubmission.ExamSubmissionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examSubmission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamSubmissionExists(examSubmission.ExamSubmissionId))
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
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Id", examSubmission.DepartmentId);
            return View(examSubmission);
        }

        // GET: ExamSubmissions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examSubmission = await _context.ExamSubmission
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.ExamSubmissionId == id);
            if (examSubmission == null)
            {
                return NotFound();
            }

            return View(examSubmission);
        }

        // POST: ExamSubmissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var examSubmission = await _context.ExamSubmission.FindAsync(id);
            if (examSubmission != null)
            {
                _context.ExamSubmission.Remove(examSubmission);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamSubmissionExists(int id)
        {
            return _context.ExamSubmission.Any(e => e.ExamSubmissionId == id);
        }
    }
}
