using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using JobPortal1.Models;
using System.Linq;

namespace JobPortal1.Controllers
{
    public class JobSeekerDashboardController : Controller
    {
        private readonly JobPortalTester1Context _context;

        public JobSeekerDashboardController(JobPortalTester1Context context)
        {
            _context = context;
        }

        // GET: Dashboard/Index (Displays the JobSeeker Dashboard)
        public async Task<IActionResult> Index(int userId)
        {
            // Get the JobSeeker info
            var jobSeeker = await _context.JobSeekers
                .Include(js => js.JobSeekerPersonalInfos) // Ensure PersonalInfo is loaded
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (jobSeeker == null)
            {
                return NotFound();
            }

            // Get relevant notifications for the jobseeker
            var notifications = await _context.Notifications
                .Where(n => n.JobSeekerId == jobSeeker.JobSeekerId && n.NotificationType == "JobseekerNotification")
                .OrderBy(n => n.IsRead) // Unread notifications first
                .ThenByDescending(n => n.CreatedAt) // Sort by creation date
                .ToListAsync();

            ViewBag.Notifications = notifications;

            return View(jobSeeker);
        }

        // POST: Dashboard/MarkAsRead
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int notificationId, int userId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);

            if (notification != null)
            {
                notification.IsRead = true;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }

            // Redirect back to the dashboard with the userId
            return RedirectToAction("Index", new { userId = userId });
        }
    }
}
