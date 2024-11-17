using JobPortal1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JobPortal1.Controllers
    {
    public class EmployerRegisterController : Controller
        {
        private readonly JobPortalTester1Context _context;

        public EmployerRegisterController(JobPortalTester1Context context)
            {
            _context = context;
            }

        [HttpGet]
        public IActionResult Create(int userId)
            {
            // Initialize an employer model with the UserId to link it to the User
            var employer = new Employer
                {
                UserId = userId,  // Link to the created UserId
                EmployerType = "Company" // Default to Company
                };

            return View(employer);
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employer employer)
            {
            if (ModelState.IsValid)
                {
                // Set "N/A" for private employers
                if (employer.EmployerType == "Private")
                    {
                    employer.CompanyName = "N/A";
                    employer.Industry = "N/A";
                    employer.City = "N/A";
                    employer.Province = "N/A";
                    employer.Suburb = "N/A";
                    }

                // Save the employer details
                _context.Employers.Add(employer);
                await _context.SaveChangesAsync();

                // Redirect to EmployerDashboard once saved
                return RedirectToAction("Index", "EmployerDashboard", new { userId = employer.UserId });
                }

            // Return the view with validation errors
            return View(employer);
            }
        }
    }