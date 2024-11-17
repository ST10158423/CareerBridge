using JobPortal1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace JobPortal1.Controllers
{
    public class EmployerSettingsController : Controller
    {
        private readonly JobPortalTester1Context _context;

        public EmployerSettingsController(JobPortalTester1Context context)
        {
            _context = context;
        }

        // GET: EmployerSettings/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int employerId)
        {
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.EmployerId == employerId);
            if (employer == null)
            {
                return NotFound("Employer not found");
            }

            return View(employer);
        }

        // POST: EmployerSettings/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employer employer)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the existing Employer record to ensure the UserId is retained
                var existingEmployer = await _context.Employers.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EmployerId == employer.EmployerId);

                if (existingEmployer == null)
                {
                    return NotFound("Employer not found");
                }

                // Update the Employer record and retain the existing UserId
                employer.UserId = existingEmployer.UserId;

                _context.Update(employer);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "EmployerDashboard", new { userId = employer.UserId });
            }

            return View(employer); // Return to view if there are validation errors
        }
    }
}
