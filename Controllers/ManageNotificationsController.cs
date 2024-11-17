using JobPortal1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortal1.Controllers
{
    public class ManageNotificationsController : Controller
    {
        private readonly JobPortalTester1Context _context;

        public ManageNotificationsController(JobPortalTester1Context context)
        {
            _context = context;
        }

        // GET: Manage Notifications page
        [HttpGet]
        [Route("ManageNotifications")]
        public async Task<IActionResult> Index()
        {
            // Fetch only unread notifications with NotificationType "Payment"
            var notifications = await _context.Notifications
                .Where(n => n.NotificationType == "Payment" && n.IsRead == false) // Only unread Payment notifications
                .ToListAsync();

            return View(notifications);
        }

        // POST: Send Notification to Job Seeker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendNotification(int notificationId, int jobSeekerId, string message)
        {
            // Retrieve the original notification
            var originalNotification = await _context.Notifications.FindAsync(notificationId);
            if (originalNotification == null)
            {
                return NotFound();
            }

            // Get the current admin's UserId (in a real application, this would be obtained from the logged-in user's context)
            int adminUserId = 1; // Replace with logic to retrieve the actual admin ID

            // Create a new notification to send to the JobSeeker
            var newNotification = new Notification
            {
                UserId = adminUserId,
                JobSeekerId = jobSeekerId,

                Message = message,
                NotificationType = "JobseekerNotification",
                IsRead = false,
                CreatedAt = DateTime.Now,
                IsActionRequired = true
            };

            _context.Notifications.Add(newNotification);

            // Mark the original notification as read and update necessary fields
            originalNotification.IsRead = true;
            originalNotification.ReadAt = DateTime.Now;
            originalNotification.ProcessedByAdminId = adminUserId;
            originalNotification.UserId = adminUserId;

            // Update the notification in the database
            _context.Notifications.Update(originalNotification);

            await _context.SaveChangesAsync();

            // Redirect to Manage Notifications after sending
            return RedirectToAction("Index");
        }
    }
}
