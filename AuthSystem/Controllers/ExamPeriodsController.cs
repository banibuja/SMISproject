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

    public class ExamPeriodsController : Controller
    {
        private readonly AuthDbContext _context;

        public ExamPeriodsController(AuthDbContext context)
        {
            _context = context;
        }

        // GET: ExamPeriods
        public async Task<IActionResult> Index()
        {
            var authDbContext = _context.ExamPeriods.Include(e => e.Department);
            return View(await authDbContext.ToListAsync());
        }

        // GET: ExamPeriods/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examPeriod = await _context.ExamPeriods
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examPeriod == null)
            {
                return NotFound();
            }

            return View(examPeriod);
        }

        // GET: ExamPeriods/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Name");
            return View();
        }

        // POST: ExamPeriods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,EndDate,DepartmentId")] ExamPeriod examPeriod)
        {
            if (ModelState.IsValid)
            {
                _context.Add(examPeriod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Name", examPeriod.DepartmentId);
            return View(examPeriod);
        }

        // GET: ExamPeriods/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examPeriod = await _context.ExamPeriods.FindAsync(id);
            if (examPeriod == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Id", examPeriod.DepartmentId);
            return View(examPeriod);
        }

        // POST: ExamPeriods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,EndDate,DepartmentId")] ExamPeriod examPeriod)
        {
            if (id != examPeriod.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examPeriod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamPeriodExists(examPeriod.Id))
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
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "Id", examPeriod.DepartmentId);
            return View(examPeriod);
        }

        // GET: ExamPeriods/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examPeriod = await _context.ExamPeriods
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examPeriod == null)
            {
                return NotFound();
            }

            return View(examPeriod);
        }

        // POST: ExamPeriods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var examPeriod = await _context.ExamPeriods.FindAsync(id);
            if (examPeriod != null)
            {
                _context.ExamPeriods.Remove(examPeriod);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamPeriodExists(int id)
        {
            return _context.ExamPeriods.Any(e => e.Id == id);
        }
    }
}
