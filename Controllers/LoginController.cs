using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using JobPortal1.Models;
using BCrypt.Net;
using Firebase.Auth;

namespace JobPortal1.Controllers
{
    public class LoginController : Controller
    {
        private readonly JobPortalTester1Context _context;
        private readonly FirebaseAuthProvider auth;

        public LoginController(JobPortalTester1Context context)
        {
            _context = context;
            auth = new FirebaseAuthProvider(new FirebaseConfig(("AIzaSyDQG5B0Sb0HRuj2I_DEVhjhs3RxLy-p0nE")));
        }

        // GET: Login
        public IActionResult Index()
        {
            return View(); // Returns the login view
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(JobPortal1.Models.User model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && !string.IsNullOrWhiteSpace(model.PasswordHash))
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

                try
                {
                    // Firebase Login
                    var firebaseAuthLink = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.PasswordHash);

                    // Check if the user exists in the local database and update password hash
                    if (user != null)
                    {
                        if (!BCrypt.Net.BCrypt.Verify(model.PasswordHash, user.PasswordHash))
                        {
                            // Update local database with Firebase password hash
                            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);
                        }

                        user.LastLogin = DateTime.Now;


                        _context.Update(user);
                        await _context.SaveChangesAsync();

                        // Update Session
                        HttpContext.Session.SetString("UserRole", user.UserType);
                        HttpContext.Session.SetInt32("UserId", user.UserId);


                        // Redirect based on user type
                        if (user.UserType == "JobSeeker")
                        {
                            HttpContext.Session.SetInt32("JobSeekerId", user.UserId);
                            return RedirectToAction("Index", "JobSeekerDashboard", new { userId = user.UserId });
                        }

                        if (user.UserType == "Employer")
                        {
                            HttpContext.Session.SetInt32("EmployerId", user.UserId);
                            return RedirectToAction("Index", "EmployerDashboard", new { userId = user.UserId });
                        }

                        if (user.UserType == "Admin")
                        {
                            return RedirectToAction("Index", "AdminDashboard");
                        }
                    }
                    else
                    {
                        TempData["LoginError"] = "Invalid email or password.";
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    TempData["LoginError"] = "Invalid email or password.";
                }
            }
            else
            {
                TempData["LoginError"] = "Email and Password are required.";
            }

            return View(model);

            
        }
        // GET: Forgot Password
        public IActionResult ForgotPassword()
        {
            return View(); // Returns the Forgot Password view
        }

        // POST: Forgot Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    await auth.SendPasswordResetEmailAsync(email);
                    TempData["ResetMessage"] = "Password reset link sent to your email.";
                }
                catch (FirebaseAuthException ex)
                {
                    TempData["ResetError"] = $"Firebase error: {ex.Message}";
                }
                catch (Exception ex)
                {
                    TempData["ResetError"] = $"An error occurred: {ex.Message}";
                }
            }
            else
            {
                TempData["ResetError"] = "Email is required.";
            }

            return RedirectToAction("Index");
        }

        // GET: Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
