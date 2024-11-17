using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobPortal1.Models;
using System.Threading.Tasks;
using System.Linq;

namespace JobPortal1.Controllers
{
    public class JobSeekerWorkExpController : Controller
    {
        private readonly JobPortalTester1Context _context;

        public JobSeekerWorkExpController(JobPortalTester1Context context)
        {
            _context = context;
        }

        // GET: WorkExperience/Create (Display initial work experience entry form)
        public async Task<IActionResult> Create(int jobSeekerId)
        {
            ViewBag.JobSeekerId = jobSeekerId;
            var workExperience = await _context.JobSeekerWorkExperiences
                .Where(w => w.JobSeekerId == jobSeekerId)
                .ToListAsync();

            ViewBag.WorkExperience = workExperience;
            return View();
        }

        // GET: WorkExperience/EditWorkExperience (Display edit form with existing experiences)
        public async Task<IActionResult> EditWorkExperience(int jobSeekerId)
        {
            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(js => js.JobSeekerId == jobSeekerId);

            if (jobSeeker == null)
            {
                return NotFound("Job Seeker not found.");
            }

            // Set JobSeekerId and UserId in ViewBag
            ViewBag.JobSeekerId = jobSeekerId;
            ViewBag.UserId = jobSeeker.UserId;

            var workExperience = await _context.JobSeekerWorkExperiences
                .Where(w => w.JobSeekerId == jobSeekerId)
                .ToListAsync();

            return View("EditWorkExperience", workExperience);
        }

        // POST: AddExperience (Add new work experience via AJAX)
        [HttpPost]
        public async Task<IActionResult> AddExperience(JobSeekerWorkExperience workExperience)
        {
            if (ModelState.IsValid)
            {
                _context.JobSeekerWorkExperiences.Add(workExperience);
                await _context.SaveChangesAsync();

                return Json(new { expId = workExperience.ExpId, companyName = workExperience.CompanyName, companyPhoneNumber = workExperience.CompanyPhoneNumber });
            }
            return BadRequest("Error adding work experience");
        }

        // POST: RemoveExperience (Remove an experience via AJAX)
        [HttpPost]
        public async Task<IActionResult> RemoveExperience(int expId)
        {
            var experience = await _context.JobSeekerWorkExperiences.FindAsync(expId);
            if (experience != null)
            {
                _context.JobSeekerWorkExperiences.Remove(experience);
                await _context.SaveChangesAsync();
                return Json(new { success = true, expId });
            }
            return BadRequest("Error removing work experience");
        }

        // POST: CompleteExperience (Finalize the editing process and redirect to profile)
        [HttpPost]
        public async Task<IActionResult> CompleteExperience(int jobSeekerId)
        {
            // Fetch the UserId from the JobSeeker table
            var jobSeeker = await _context.JobSeekers
                .Where(js => js.JobSeekerId == jobSeekerId)
                .FirstOrDefaultAsync();

            ViewBag.UserId = jobSeeker.UserId;
            return RedirectToAction("Index", "JobSeekerProfile", new { userId = jobSeeker.UserId });
        }
    }
}
