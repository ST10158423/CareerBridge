using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobPortal1.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JobPortal1.Controllers
{
    public class JobSeekerCertificateController : Controller
    {
        private readonly JobPortalTester1Context _context;
        private readonly string _uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/certificates");

        public JobSeekerCertificateController(JobPortalTester1Context context)
        {
            _context = context;
            // Ensure the upload directory exists
            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }
        }

        // GET: Certificates/Create (Display form for adding certificates)
        public async Task<IActionResult> Create(int jobSeekerId)
        {
            var jobSeeker = await _context.JobSeekers.FirstOrDefaultAsync(js => js.JobSeekerId == jobSeekerId);

            if (jobSeeker == null)
            {
                return NotFound("Job Seeker not found.");
            }

            ViewBag.JobSeekerId = jobSeekerId;
            ViewBag.UserId = jobSeeker.UserId;

            var certificates = await _context.JobSeekerCertificates
                .Where(c => c.JobSeekerId == jobSeekerId)
                .ToListAsync();

            ViewBag.Certificates = certificates;
            return View();
        }

        // GET: Certificates/EditCertificates (Manage certificates)
        public async Task<IActionResult> EditCertificates(int jobSeekerId)
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


            var certificates = await _context.JobSeekerCertificates
                .Where(c => c.JobSeekerId == jobSeekerId)
                .ToListAsync();
            

            return View("EditCertificates", certificates);
        }

        // POST: Certificates/AddCertificate (Add new certificate using AJAX)
        [HttpPost]
        public async Task<IActionResult> AddCertificate(int jobSeekerId, IFormFile certificateFile)
        {
            if (certificateFile == null || certificateFile.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            // Generate a unique file name to avoid overwriting
            var fileName = $"{Path.GetFileNameWithoutExtension(certificateFile.FileName)}_{jobSeekerId}{Path.GetExtension(certificateFile.FileName)}";
            var filePath = Path.Combine(_uploadDirectory, fileName);

            try
            {
                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await certificateFile.CopyToAsync(stream);
                }

                // Save the certificate data in the database
                var certificate = new JobSeekerCertificate
                {
                    JobSeekerId = jobSeekerId,
                    DocName = fileName,
                    DocUrl = $"/uploads/certificates/{fileName}" // Store as a relative path
                };

                _context.JobSeekerCertificates.Add(certificate);
                await _context.SaveChangesAsync();

                // Return the certificate details to update the view dynamically
                return Json(new { docId = certificate.SupDocumentsId, docName = certificate.DocName });
            }
            catch (Exception ex)
            {
                // Log the exception and return a BadRequest
                Console.WriteLine("Error uploading certificate: " + ex.Message);
                return BadRequest("Error uploading certificate");
            }
        }

        // POST: Certificates/RemoveCertificate (Remove a certificate using AJAX)
        [HttpPost]
        public async Task<IActionResult> RemoveCertificate(int docId)
        {
            var certificate = await _context.JobSeekerCertificates.FindAsync(docId);
            if (certificate == null)
            {
                return BadRequest("Certificate not found.");
            }

            // Delete the certificate from the database
            _context.JobSeekerCertificates.Remove(certificate);
            await _context.SaveChangesAsync();

            // Delete the file from the server if it exists
            var fullPath = Path.Combine("wwwroot", certificate.DocUrl.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            return Json(new { success = true, docId });
        }

        // POST: Certificates/CompleteCertificates (Finalize and redirect to JobSeeker Profile)
        [HttpPost]
        public async Task<IActionResult> CompleteCertificates(int jobSeekerId)
        {
            // Fetch the UserId from the JobSeeker table
            var jobSeeker = await _context.JobSeekers
                .Where(js => js.JobSeekerId == jobSeekerId)
                .FirstOrDefaultAsync();

            ViewBag.UserId = jobSeeker.UserId;

            // Redirect to the JobSeekerDashboard with UserId
            return RedirectToAction("Index", "JobSeekerDashboard", new { userId = jobSeeker.UserId });
        }

    }
}
