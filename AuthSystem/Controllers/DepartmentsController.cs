using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthSystem.Data;
using AuthSystem.Models;
using Microsoft.AspNetCore.Identity;
using AuthSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace AuthSystem.Controllers
{
    [Authorize(Roles = "Admin")]

    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepartmentsController(AuthDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task LogAction(string action, string entity, int entityId, string userId, string details = "")
        {
            var log = new Log
            {
                Action = action,
                Entity = entity,  
                EntityId = entityId,  
                UserId = userId,  
                Timestamp = DateTime.UtcNow,  
                Details = details  
            };

            _context.Add(log);  
            await _context.SaveChangesAsync();  
        }


        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Department.ToListAsync();
            return View(departments);
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Department
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();

                // Regjistro log-un
                var userId = _userManager.GetUserId(User);  // Get the current user's ID
                await LogAction("Create", "Department", department.Id, userId, $"Created department: {department.Name}");

                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();

                    // Regjistro log-un
                    var userId = _userManager.GetUserId(User);
                    await LogAction("Update", "Department", department.Id, userId, $"Updated department name to: {department.Name}");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
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
            return View(department);
        }


        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Department
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Department.FindAsync(id);
            if (department != null)
            {
                _context.Department.Remove(department);

                // Log the delete action
                var userId = _userManager.GetUserId(User);
                await LogAction("Delete", "Department", department.Id, userId, $"Deleted department: {department.Name}");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.Id == id);
        }
        private async Task<Department> GetDepartmentById(int id)
        {
            return await _context.Department.FirstOrDefaultAsync(m => m.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult>  GetAll()
        {
            var departments = await _context.Department.ToListAsync();
            return Ok(departments);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department = await GetDepartmentById(id);
            if (department == null)
            {
                return NotFound(new { Message = "Department not found!" });
            }
            return Ok(new { Department = department, Message = "Fetched successfully!" });
        }


    }
}
