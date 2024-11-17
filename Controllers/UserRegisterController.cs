using Microsoft.AspNetCore.Mvc;
using JobPortal1.Models;
using System.Threading.Tasks;
using Firebase.Auth;
using Microsoft.EntityFrameworkCore;

namespace JobPortal1.Controllers
{
    public class UserRegisterController : Controller
    {
        private readonly JobPortalTester1Context _context;
        private readonly FirebaseAuthProvider auth;

        public UserRegisterController(JobPortalTester1Context context)
        {
            _context = context;
            auth = new FirebaseAuthProvider(new FirebaseConfig(("AIzaSyDQG5B0Sb0HRuj2I_DEVhjhs3RxLy-p0nE")));
        }

        // GET: UserRegister/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserRegister/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,PasswordHash,Email,UserType")] JobPortal1.Models.User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if username or email already exists
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Username == user.Username || u.Email == user.Email);

                    if (existingUser != null)
                    {
                        if (existingUser.Username == user.Username)
                        {
                            ModelState.AddModelError("Username", "The username is already taken. Please choose a different username.");
                        }

                        if (existingUser.Email == user.Email)
                        {
                            ModelState.AddModelError("Email", "The email is already registered. Please use a different email.");
                        }

                        // Return the view with errors
                        return View(user);
                    }

                    // Register user in Firebase
                    await auth.CreateUserWithEmailAndPasswordAsync(user.Email, user.PasswordHash);

                    // Hash password before saving to SSMS
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                    user.IsActive = true;
                    user.DateCreated = DateTime.Now;

                    _context.Add(user);
                    await _context.SaveChangesAsync();

                    HttpContext.Session.SetString("UserRole", user.UserType);
                    HttpContext.Session.SetInt32("UserId", user.UserId);

                    if (user.UserType == "JobSeeker")
                    {
                        HttpContext.Session.SetInt32("JobSeekerId", user.UserId);
                        return RedirectToAction("Create", "JobSeekerRegister", new { userId = user.UserId });
                    }


                    if (user.UserType == "Employer")
                    {
                        HttpContext.Session.SetInt32("EmployerId", user.UserId);
                        return RedirectToAction("Create", "EmployerRegister", new { userId = user.UserId });
                    }

                }
                catch (Exception ex)
                {
                    TempData["RegisterError"] = "Error with registering";
                }
            }

            // If model state is not valid, return the view with the user model to show errors
            return View(user);
        }
    }
}
