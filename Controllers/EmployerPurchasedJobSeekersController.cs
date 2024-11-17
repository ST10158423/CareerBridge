using JobPortal1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortal1.Controllers
{
    public class EmployerPurchasedJobSeekersController : Controller
    {
        private readonly JobPortalTester1Context _context;

        public EmployerPurchasedJobSeekersController(JobPortalTester1Context context)
        {
            _context = context;
        }

        // GET: Previously purchased job seekers
        public async Task<IActionResult> Index(int employerId)
        {
            // Fetch the employer and user ID
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.EmployerId == employerId);
            if (employer == null)
            {
                return NotFound("Employer not found.");
            }

            // Store userId in ViewBag
            ViewBag.UserId = employer.UserId;

            var purchasedJobSeekers = await _context.Payments
                .Include(p => p.Interest)
                .ThenInclude(i => i.JobSeeker)
                .Where(p => p.Interest.EmployerId == employerId && p.PaymentStatus == "Success")
                .Select(p => p.Interest.JobSeeker)
                .Distinct()
                .ToListAsync();

            return View(purchasedJobSeekers);
        }

        // POST: Download PDF for a specific job seeker
        [HttpPost]
        public async Task<IActionResult> DownloadJobSeekerInfo(int jobSeekerId)
        {
            var jobSeeker = await _context.JobSeekers
                .Include(js => js.JobSeekerPersonalInfos)
                .Include(js => js.JobSeekerSkills)
                .Include(js => js.JobSeekerCertificates)
                .Include(js => js.JobSeekerWorkExperiences)
                .Join(_context.Users,
                      js => js.UserId,
                      u => u.UserId,
                      (js, u) => new { JobSeeker = js, User = u })
                .FirstOrDefaultAsync(jsu => jsu.JobSeeker.JobSeekerId == jobSeekerId);

            if (jobSeeker == null)
            {
                return NotFound("Job Seeker not found.");
            }

            var jobSeekerData = jobSeeker.JobSeeker;
            var userData = jobSeeker.User;

            using (var memoryStream = new MemoryStream())
            {
                var pdf = new PdfDocument();
                var page = pdf.AddPage();
                var graphics = XGraphics.FromPdfPage(page);
                var font = new XFont("Arial", 12, XFontStyle.Regular);
                var boldFont = new XFont("Arial", 14, XFontStyle.Bold);
                double yPosition = 40; 
                const double lineHeight = 20; 

                // Title
                graphics.DrawString("Job Seeker Full Profile", new XFont("Arial", 18, XFontStyle.Bold), XBrushes.Black, new XPoint(40, yPosition));
                yPosition += lineHeight * 2;

                // Job Seeker Details
                graphics.DrawString($"Name: {jobSeekerData.FirstName} {jobSeekerData.LastName}", font, XBrushes.Black, new XPoint(40, yPosition));
                yPosition += lineHeight;
                graphics.DrawString($"Phone Number: {jobSeekerData.PhoneNumber ?? "Not provided"}", font, XBrushes.Black, new XPoint(40, yPosition));
                yPosition += lineHeight;
                graphics.DrawString($"Email: {userData.Email}", font, XBrushes.Black, new XPoint(40, yPosition));
                yPosition += lineHeight;

                // Personal Information
                var personalInfo = jobSeekerData.JobSeekerPersonalInfos.FirstOrDefault();
                if (personalInfo != null)
                {
                    graphics.DrawString($"City: {personalInfo.City}", font, XBrushes.Black, new XPoint(40, yPosition));
                    yPosition += lineHeight;
                    graphics.DrawString($"Province: {personalInfo.Province}", font, XBrushes.Black, new XPoint(40, yPosition));
                    yPosition += lineHeight;
                    graphics.DrawString($"Suburb: {personalInfo.Suburb}", font, XBrushes.Black, new XPoint(40, yPosition));
                    yPosition += lineHeight;
                    graphics.DrawString($"Education: {personalInfo.LevelOfEducation}", font, XBrushes.Black, new XPoint(40, yPosition));
                    yPosition += lineHeight;
                    graphics.DrawString($"About You: {personalInfo.AboutYou}", font, XBrushes.Black, new XPoint(40, yPosition));
                    yPosition += lineHeight * 2;
                }

                // Skills
                graphics.DrawString("Skills:", boldFont, XBrushes.Black, new XPoint(40, yPosition));
                yPosition += lineHeight;
                if (jobSeekerData.JobSeekerSkills.Any())
                {
                    foreach (var skill in jobSeekerData.JobSeekerSkills)
                    {
                        graphics.DrawString($"- {skill.SkillName} (Level: {skill.SkillLevel})", font, XBrushes.Black, new XPoint(40, yPosition));
                        yPosition += lineHeight;
                    }
                }
                else
                {
                    graphics.DrawString("No skills listed", font, XBrushes.Black, new XPoint(40, yPosition));
                    yPosition += lineHeight;
                }

                // Work Experience
                graphics.DrawString("Work Experience:", boldFont, XBrushes.Black, new XPoint(40, yPosition));
                yPosition += lineHeight;
                if (jobSeekerData.JobSeekerWorkExperiences.Any())
                {
                    foreach (var exp in jobSeekerData.JobSeekerWorkExperiences)
                    {
                        graphics.DrawString($"- Company: {exp.CompanyName}", font, XBrushes.Black, new XPoint(40, yPosition));
                        yPosition += lineHeight;
                        graphics.DrawString($"  Address: {exp.CompanyAddress}", font, XBrushes.Black, new XPoint(40, yPosition));
                        yPosition += lineHeight;
                        graphics.DrawString($"  Phone: {exp.CompanyPhoneNumber}", font, XBrushes.Black, new XPoint(40, yPosition));
                        yPosition += lineHeight * 2;
                    }
                }
                else
                {
                    graphics.DrawString("No work experience listed", font, XBrushes.Black, new XPoint(40, yPosition));
                    yPosition += lineHeight;
                }

                

                // Save the PDF
                pdf.Save(memoryStream, false);
                return File(memoryStream.ToArray(), "application/pdf", $"{jobSeekerData.FirstName}_{jobSeekerData.LastName}_Profile.pdf");
            }
        }

    }
}
