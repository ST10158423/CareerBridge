using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobPortal1.Models;
using System.Threading.Tasks;
using System.Linq;

namespace JobPortal1.Controllers
{
    public class SkillsController : Controller
    {
        private readonly JobPortalTester1Context _context;

        public SkillsController(JobPortalTester1Context context)
        {
            _context = context;
        }

        // GET: Skills/Create (Display the Skills form and existing skills)
        public async Task<IActionResult> Create(int jobSeekerId)
        {
            ViewBag.JobSeekerId = jobSeekerId;
            var skills = await _context.JobSeekerSkills
                .Where(s => s.JobSeekerId == jobSeekerId)
                .ToListAsync();

            ViewBag.Skills = skills;
            return View();
        }

        // GET: Skills/EditSkills (Display skills for editing)
        public async Task<IActionResult> EditSkills(int jobSeekerId)
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

            var skills = await _context.JobSeekerSkills
                .Where(s => s.JobSeekerId == jobSeekerId)
                .ToListAsync();

            // If no skills are found, initialize an empty list
            if (skills == null || !skills.Any())
            {
                skills = new List<JobSeekerSkill>();
            }

            return View(skills);  // Passing the skills model to the view
        }

        // POST: Skills/AddSkill (Add a new skill using AJAX)
        [HttpPost]
        public async Task<IActionResult> AddSkill(JobSeekerSkill skill)
        {
            if (ModelState.IsValid)
            {
                // Add the new skill to the database
                _context.JobSeekerSkills.Add(skill);
                await _context.SaveChangesAsync();

                // Return the added skill as a JSON response to be appended on the front-end
                return Json(new { skillId = skill.SkillId, skillName = skill.SkillName, skillLevel = skill.SkillLevel });
            }

            return BadRequest("Error adding skill");
        }

        // POST: Skills/RemoveSkill (Remove a skill using AJAX)
        [HttpPost]
        public async Task<IActionResult> RemoveSkill(int skillId)
        {
            var skill = await _context.JobSeekerSkills.FindAsync(skillId);
            if (skill != null)
            {
                _context.JobSeekerSkills.Remove(skill);
                await _context.SaveChangesAsync();
                return Json(new { success = true, skillId });
            }

            return BadRequest("Error removing skill");
        }

        // POST: Skills/CompleteSkills (Redirect after saving skills)
        [HttpPost]
        public IActionResult CompleteSkills(int jobSeekerId)
        {
            // Redirect to JobSeekerWorkExp after saving the skills
            return RedirectToAction("Create", "JobSeekerWorkExp", new { jobSeekerId });
        }

        // POST: Skills/SaveEditedSkills (Save the updated skills for a jobseeker)
        [HttpPost]
        public async Task<IActionResult> SaveEditedSkills(List<JobSeekerSkill> skills, int jobSeekerId)
        {
            // Retrieve existing skills for the job seeker
            var existingSkills = await _context.JobSeekerSkills
                .Where(s => s.JobSeekerId == jobSeekerId)
                .ToListAsync();

            // Remove existing skills not in the new list
            foreach (var existingSkill in existingSkills)
            {
                if (!skills.Any(s => s.SkillId == existingSkill.SkillId))
                {
                    _context.JobSeekerSkills.Remove(existingSkill);
                }
            }

            // Update existing skills or add new skills
            foreach (var skill in skills)
            {
                if (skill.SkillId == 0)
                {
                    // New skill
                    skill.JobSeekerId = jobSeekerId;
                    _context.JobSeekerSkills.Add(skill);
                }
                else
                {
                    // Existing skill
                    var existingSkill = existingSkills.FirstOrDefault(s => s.SkillId == skill.SkillId);
                    if (existingSkill != null)
                    {
                        existingSkill.SkillName = skill.SkillName;
                        existingSkill.SkillLevel = skill.SkillLevel;
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Skills updated successfully!" });
        }
    }
}
