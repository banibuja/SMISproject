using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthSystem.Data;
using AuthSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Org.BouncyCastle.Utilities;

namespace AuthSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LogsController : Controller
    {
        private readonly AuthDbContext _context;

        public LogsController(AuthDbContext context)
        {
            _context = context;
        }

        // GET: Logs
        public async Task<IActionResult> Index()
        {
            var logs = await _context.Logs
                .Include(log => log.User)
                .ToListAsync();

            return View("~/Views/AllLogs/Index.cshtml", logs);
        }

        // GET: Logs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs
                .FirstOrDefaultAsync(m => m.Id == id);

            if (log == null)
            {
                return NotFound();
            }

            return View("~/Views/AllLogs/Details.cshtml", log);
        }

        // GET: Logs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs
                .FirstOrDefaultAsync(m => m.Id == id);

            if (log == null)
            {
                return NotFound();
            }

            return View("~/Views/AllLogs/Delete.cshtml", log);
        }

        // POST: Logs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var log = await _context.Logs.FindAsync(id);
            if (log != null)
            {
                // Create a log entry for the deletion
                var deletionLog = new Log
                {
                    Action = "Deleted",
                    Entity = "EntityName",
                    EntityId = id,
                    UserId = "CurrentUserId", // Replace with the actual current user's ID
                    Timestamp = DateTime.Now,
                    Details = $"Log entry with ID {id} has been deleted."
                };

                _context.Logs.Remove(log);
                _context.Logs.Add(deletionLog); // Save the deletion log
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LogExists(int id)
        {
            return _context.Logs.Any(e => e.Id == id);
        }
    }
}
