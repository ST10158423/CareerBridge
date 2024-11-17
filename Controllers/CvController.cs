using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobPortal1.Models;
using System.Threading.Tasks;

namespace JobPortal1.Controllers
{
    public class CVController : Controller
    {
        private readonly JobPortalTester1Context _context;

        public CVController(JobPortalTester1Context context)
        {
            _context = context;
        }

        // GET: CV/PersonalInfo (Load the personal info section of the CV)
        public async Task<IActionResult> Create(int jobSeekerId)
        {
            // Get the JobSeeker details
            var jobSeeker = await _context.JobSeekers.FirstOrDefaultAsync(js => js.JobSeekerId == jobSeekerId);

            if (jobSeeker == null)
            {
                return NotFound();
            }

            // Retrieve the personal info, or initialize it if not present
            var personalInfo = await _context.JobSeekerPersonalInfos
                .FirstOrDefaultAsync(pi => pi.JobSeekerId == jobSeekerId)
                ?? new JobSeekerPersonalInfo
                {
                    JobSeekerId = jobSeeker.JobSeekerId,
                    FirstName = jobSeeker.FirstName,
                    LastName = jobSeeker.LastName,
                    PhoneNumber = jobSeeker.PhoneNumber
                };

            ViewBag.JobSeekerId = jobSeeker.JobSeekerId;

            return View(personalInfo);
        }

        // POST: CV/PersonalInfo (Save personal info and redirect to JobSeekerSkills)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersonalInfo(JobSeekerPersonalInfo personalInfo)
        {
            if (ModelState.IsValid)
            {
                // Add or update the JobSeekerPersonalInfo
                if (personalInfo.PersonalInfoId == 0)
                {
                    _context.JobSeekerPersonalInfos.Add(personalInfo);
                }
                else
                {
                    _context.JobSeekerPersonalInfos.Update(personalInfo);
                }

                await _context.SaveChangesAsync();

                // Redirect to the Skills page after saving personal info
                return RedirectToAction("Create", "Skills", new { jobSeekerId = personalInfo.JobSeekerId });
            }

            ViewBag.JobSeekerId = personalInfo.JobSeekerId;
            return View(personalInfo);
        }
    }
}
