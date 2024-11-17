using Microsoft.AspNetCore.Mvc;

namespace JobPortal1.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ManageUsers()
        {
            // This should load the Manage Users view
            return RedirectToAction("Index", "ManageUsers");
        }
        public IActionResult ManageNotifications()
        {
            // This should load the Manage Notifications view
            return RedirectToAction("Index", "ManageNotifications");
        }
    }
}
