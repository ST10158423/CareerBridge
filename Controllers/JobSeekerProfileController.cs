using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobPortal1.Models;
using System.Threading.Tasks;

namespace JobPortal1.Controllers
{
    public class JobSeekerProfileController : Controller
    {
        private readonly JobPortalTester1Context _context;

        public JobSeekerProfileController(JobPortalTester1Context context)
        {
            _context = context;
        }

        // GET: Profile (Displays the jobseeker profile)
        public async Task<IActionResult> Index(int userId)
        {
            var jobSeeker = await _context.JobSeekers
                .Include(js => js.JobSeekerPersonalInfos)
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (jobSeeker == null)
            {
                return NotFound();
            }

            return View(jobSeeker);
        }

        // POST: Profile/SaveProfile (Handles form submission and saves changes to both JobSeeker and JobSeekerPersonalInfo)
        [HttpPost]
        public async Task<IActionResult> SaveProfile(int userId, JobSeekerPersonalInfo personalInfo, string firstName, string lastName, string phoneNumber)
        {
            // Retrieve JobSeeker
            var jobSeeker = await _context.JobSeekers
                .Include(js => js.JobSeekerPersonalInfos)
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (jobSeeker == null)
            {
                return Json(new { success = false, message = "Job Seeker not found." });
            }

            // Update JobSeeker fields
            jobSeeker.FirstName = firstName;
            jobSeeker.LastName = lastName;
            jobSeeker.PhoneNumber = phoneNumber;

            // Update JobSeekerPersonalInfo
            var existingPersonalInfo = jobSeeker.JobSeekerPersonalInfos.FirstOrDefault();
            if (existingPersonalInfo != null)
            {
                existingPersonalInfo.FirstName = firstName; // Sync with JobSeeker
                existingPersonalInfo.LastName = lastName;   // Sync with JobSeeker
                existingPersonalInfo.PhoneNumber = phoneNumber;  // Sync with JobSeeker
                existingPersonalInfo.City = personalInfo.City;
                existingPersonalInfo.Province = personalInfo.Province;
                existingPersonalInfo.Suburb = personalInfo.Suburb;
                existingPersonalInfo.AboutYou = personalInfo.AboutYou;
                existingPersonalInfo.LevelOfEducation = personalInfo.LevelOfEducation;
            }

            // Save changes to both tables
            _context.JobSeekers.Update(jobSeeker);
            await _context.SaveChangesAsync();

            // Return success response
            return Json(new { success = true, message = "Profile updated successfully!" });
        }

        // POST: Profile/ToggleEmploymentStatus (Toggles employment status via AJAX)
        [HttpPost]
        public async Task<IActionResult> ToggleEmploymentStatus(int userId, bool isEmployed)
        {
            var jobSeeker = await _context.JobSeekers.FirstOrDefaultAsync(js => js.UserId == userId);
            if (jobSeeker != null)
            {
                jobSeeker.IsEmployed = isEmployed;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
    }
}
