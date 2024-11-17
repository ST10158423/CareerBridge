using Microsoft.AspNetCore.Mvc;
using JobPortal1.Models;
using System.Threading.Tasks;

namespace JobPortal1.Controllers
{
    public class JobSeekerRegisterController : Controller
    {
        private readonly JobPortalTester1Context _context;

        public JobSeekerRegisterController(JobPortalTester1Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create(int userId)
        {
            // Create a new JobSeeker instance and set IsEmployed to false by default
            var jobSeeker = new JobSeeker
            {
                UserId = userId,
                IsEmployed = false // Automatically set IsEmployed to false
            };
            return View(jobSeeker);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobSeeker jobSeeker)
        {
            if (ModelState.IsValid)
            {
                // Add the JobSeeker to the database
                _context.JobSeekers.Add(jobSeeker);
                await _context.SaveChangesAsync();

                // Redirect to CV creation after JobSeeker registration
                return RedirectToAction("Create", "CV", new { jobSeekerId = jobSeeker.JobSeekerId });
            }

            return View(jobSeeker);
        }
    }
}
