    using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
    using JobPortal1.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using PdfSharpCore.Drawing;
    using PdfSharpCore.Pdf;
    using System.IO;
    using System.Threading.Tasks;

    namespace JobPortal1.Controllers
    {
        public class EmployerDashboardController : Controller
        {
            private readonly JobPortalTester1Context _context;
            private readonly PayFastSettings _payFastSettings;

            public EmployerDashboardController(JobPortalTester1Context context, IOptions<PayFastSettings> payFastSettings)
            {
                _context = context;
                _payFastSettings = payFastSettings.Value;
            }

        // GET: EmployerDashboard/Index
        // GET: EmployerDashboard/Index
        public async Task<IActionResult> Index(int userId, string province, string city, string suburb, string skill)
        {
            // Retrieve EmployerId using UserId
            var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employer == null)
            {
                return NotFound("Employer not found.");
            }

            int employerId = employer.EmployerId;

            // Store EmployerId and UserId separately in ViewBag
            ViewBag.EmployerId = employerId;
            ViewBag.UserId = userId;

            // Preserve filter values in ViewData
            ViewData["Province"] = province;
            ViewData["City"] = city;
            ViewData["Suburb"] = suburb;
            ViewData["Skill"] = skill;

            // Fetch all unemployed job seekers
            var jobSeekers = await _context.JobSeekers
                .Include(js => js.JobSeekerPersonalInfos)
                .Include(js => js.JobSeekerSkills)
                .Where(js => js.IsEmployed == false)
                .ToListAsync();

            // Use a HashMap to categorize job seekers by filters
            var jobSeekerMap = new Dictionary<int, JobSeeker>();

            foreach (var jobSeeker in jobSeekers)
            {
                var personalInfo = jobSeeker.JobSeekerPersonalInfos.FirstOrDefault();
                var skills = jobSeeker.JobSeekerSkills;

                // Check if job seeker matches filters
                bool matchesProvince = string.IsNullOrEmpty(province) ||
                    (personalInfo != null && personalInfo.Province != null && personalInfo.Province.ToLower().Contains(province.ToLower()));
                bool matchesCity = string.IsNullOrEmpty(city) ||
                    (personalInfo != null && personalInfo.City != null && personalInfo.City.ToLower().Contains(city.ToLower()));
                bool matchesSuburb = string.IsNullOrEmpty(suburb) ||
                    (personalInfo != null && personalInfo.Suburb != null && personalInfo.Suburb.ToLower().Contains(suburb.ToLower()));
                bool matchesSkill = string.IsNullOrEmpty(skill) ||
                    (skills.Any(s => s.SkillName != null && s.SkillName.ToLower().Contains(skill.ToLower())));

                // Add to hashmap if all filters match
                if (matchesProvince && matchesCity && matchesSuburb && matchesSkill)
                {
                    jobSeekerMap[jobSeeker.JobSeekerId] = jobSeeker;
                }
            }

            // Retrieve filtered job seekers from the hashmap
            var filteredJobSeekers = jobSeekerMap.Values.ToList();

            return View(filteredJobSeekers);
        }
        // GET: EmployerDashboard/JobSeekerDetails
        public async Task<IActionResult> JobSeekerDetails(int jobSeekerId, int employerId)
            {
                ViewBag.EmployerId = employerId;

                var jobSeeker = await _context.JobSeekers
                    .Include(js => js.JobSeekerPersonalInfos)
                    .Include(js => js.JobSeekerSkills)
                    .FirstOrDefaultAsync(js => js.JobSeekerId == jobSeekerId);

                if (jobSeeker == null) return NotFound();

                return View(jobSeeker);
            }

            // POST: EmployerDashboard/ExpressInterest
            [HttpPost]
            public async Task<IActionResult> ExpressInterest(int jobSeekerId, int employerId)
            {
                // Validate that the employer exists
                var employerExists = await _context.Employers.AnyAsync(e => e.EmployerId == employerId);
                if (!employerExists)
                {
                    return NotFound("Employer not found.");
                }

                // Check if an existing interest already exists
                var existingInterest = await _context.Interests
                    .FirstOrDefaultAsync(i => i.JobSeekerId == jobSeekerId && i.EmployerId == employerId);

                if (existingInterest != null)
                {
                    return RedirectToAction("StartPayment", new { interestId = existingInterest.InterestId });
                }

                // Create a new interest record
                var interest = new Interest
                {
                    EmployerId = employerId,
                    JobSeekerId = jobSeekerId,
                    DateExpressed = DateTime.Now,
                    PaymentStatus = "Pending"
                };

                _context.Interests.Add(interest);
                await _context.SaveChangesAsync();

                // Redirect to StartPayment action with the new interestId
                return RedirectToAction("StartPayment", new { interestId = interest.InterestId });
            }


            // Initiate PayFast Payment
            public async Task<IActionResult> StartPayment(int interestId)
            {
                var interest = await _context.Interests
                    .Include(i => i.Employer)
                    .Include(i => i.JobSeeker)
                    .FirstOrDefaultAsync(i => i.InterestId == interestId);

                if (interest == null)
                {
                    return NotFound("Interest record not found.");
                }

                var payFastUrl = GeneratePayFastUrl(interestId, 100); // Test amount

                return Redirect(payFastUrl);
            }

            // Generates PayFast URL for payment redirection
            private string GeneratePayFastUrl(int interestId, decimal amount)
            {
                // Validate PayFast settings
                if (string.IsNullOrEmpty(_payFastSettings.MerchantId) ||
                    string.IsNullOrEmpty(_payFastSettings.MerchantKey) ||
                    string.IsNullOrEmpty(_payFastSettings.SandboxUrl) ||
                    string.IsNullOrEmpty(_payFastSettings.ReturnUrl) ||
                    string.IsNullOrEmpty(_payFastSettings.CancelUrl) ||
                    string.IsNullOrEmpty(_payFastSettings.NotifyUrl))
                {
                    throw new InvalidOperationException("PayFast settings are not properly configured.");
                }

                // Generate return, cancel, and notify URLs
                var returnUrl = $"{_payFastSettings.ReturnUrl}?interestId={interestId}";
                var cancelUrl = Url.Action("PaymentCancelled", "EmployerDashboard", new { interestId }, Request.Scheme);
                var notifyUrl = _payFastSettings.NotifyUrl;

                // Build parameters
                var parameters = new Dictionary<string, string>
                {
                    { "merchant_id", _payFastSettings.MerchantId },
                    { "merchant_key", _payFastSettings.MerchantKey },
                    { "return_url", returnUrl },
                    { "cancel_url", cancelUrl },
                    { "notify_url", notifyUrl },
                    { "amount", amount.ToString("F2") },
                    { "item_name", "Access to Job Seeker Details" },
                    { "item_description", "Payment for job seeker profile access" }
                };

                // Include passphrase if not null
                if (!string.IsNullOrEmpty(_payFastSettings.Passphrase))
                {
                    parameters.Add("passphrase", _payFastSettings.Passphrase);
                }

                // Construct query string
                var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));

                return $"{_payFastSettings.SandboxUrl}?{queryString}";
            }


            // Payment success action
            // Payment success action
            public async Task<IActionResult> PaymentSuccess(int interestId)
            {
                var interest = await _context.Interests
                    .Include(i => i.JobSeeker)
                    .Include(i => i.Employer)
                    .FirstOrDefaultAsync(i => i.InterestId == interestId);

                if (interest == null)
                {
                    ViewBag.Message = "Error: Interest record not found.";
                    return View("~/Views/Payment/PaymentSuccess.cshtml");
                }

                if (interest.JobSeeker == null)
                {
                    ViewBag.Message = "Error: Job Seeker details not found.";
                    return View("~/Views/Payment/PaymentSuccess.cshtml");
                }

                if (interest.PaymentStatus != "Completed")
                {
                    // Update interest payment status
                    interest.PaymentStatus = "Completed";

                    // Create and save payment record
                    var payment = new Payment
                    {
                        InterestId = interestId,
                        PaymentAmount = 100, 
                        PaymentDate = DateTime.Now,
                        PaymentMethod = "PayFast", 
                        PaymentStatus = "Success"
                    };

                    _context.Payments.Add(payment);

                    // Create and save notification
                    var notification = new Notification
                    {
                        EmployerId = interest.EmployerId,
                        JobSeekerId = interest.JobSeekerId,
                        NotificationType = "Payment",
                        Message = $"Employer {interest.Employer.CompanyName} has paid to view {interest.JobSeeker.FirstName} {interest.JobSeeker.LastName}'s information.",
                        CreatedAt = DateTime.Now,
                        IsRead = false,
                        IsActionRequired = false
                    };

                    _context.Notifications.Add(notification);

                    // Save changes
                    await _context.SaveChangesAsync();
                }

                ViewBag.Message = "Payment was successful!";
                ViewBag.JobSeekerName = $"{interest.JobSeeker.FirstName} {interest.JobSeeker.LastName}";
                ViewBag.UserId = interest.Employer.UserId;

                return View("~/Views/Payment/PaymentSuccess.cshtml", interest); // Pass interest model to view
            }


            // Payment cancelled action
            public async Task<IActionResult> PaymentCancelled(int interestId)
            {
                var interest = await _context.Interests
                .Include(i => i.Employer)
                .FirstOrDefaultAsync(i => i.InterestId == interestId);

                if (interest == null || interest.Employer == null)
                {
                    ViewBag.Message = "Error: Employer details could not be found.";
                    return View("~/Views/Payment/PaymentCancelled.cshtml");
                }

                ViewBag.Message = "Payment was cancelled. You can try again if you wish to access the job seeker's details.";
                //ViewBag.InterestId = interestId;
                ViewBag.UserId = interest.Employer.UserId;
                return View("~/Views/Payment/PaymentCancelled.cshtml");
            }

        // Action to generate and download the job seeker’s info PDF
        public async Task<IActionResult> DownloadJobSeekerInfo(int interestId)
        {
            var interest = await _context.Interests
                .Include(i => i.JobSeeker)
                .ThenInclude(js => js.JobSeekerPersonalInfos)
                .Include(i => i.JobSeeker.JobSeekerSkills)
                .Include(i => i.JobSeeker.JobSeekerWorkExperiences)
                .Include(i => i.JobSeeker.JobSeekerCertificates)
                .Join(_context.Users,
                      i => i.JobSeeker.UserId,
                      u => u.UserId,
                      (i, u) => new { Interest = i, User = u })
                .FirstOrDefaultAsync(iu => iu.Interest.InterestId == interestId);

            if (interest == null || interest.Interest.PaymentStatus != "Completed")
            {
                return NotFound("Interest record not found or payment not completed.");
            }

            var jobSeeker = interest.Interest.JobSeeker;
            var user = interest.User;

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
                graphics.DrawString($"Name: {jobSeeker.FirstName} {jobSeeker.LastName}", font, XBrushes.Black, new XPoint(40, yPosition));
                yPosition += lineHeight;
                graphics.DrawString($"Phone Number: {jobSeeker.PhoneNumber ?? "Not provided"}", font, XBrushes.Black, new XPoint(40, yPosition));
                yPosition += lineHeight;
                graphics.DrawString($"Email: {user.Email}", font, XBrushes.Black, new XPoint(40, yPosition));
                yPosition += lineHeight;

                // Personal Information
                var personalInfo = jobSeeker.JobSeekerPersonalInfos.FirstOrDefault();
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
                if (jobSeeker.JobSeekerSkills.Any())
                {
                    foreach (var skill in jobSeeker.JobSeekerSkills)
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
                if (jobSeeker.JobSeekerWorkExperiences.Any())
                {
                    foreach (var exp in jobSeeker.JobSeekerWorkExperiences)
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

                // Save and return the PDF
                pdf.Save(memoryStream, false);
                return File(memoryStream.ToArray(), "application/pdf", $"{jobSeeker.FirstName}_{jobSeeker.LastName}_Profile.pdf");
            }
        }

    }
}
