using FirebaseAdmin.Auth;
using JobPortal1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace JobPortal1.Controllers
{
    public class ManageUsersController : Controller
    {
        private readonly JobPortalTester1Context _context;

        public ManageUsersController(JobPortalTester1Context context)
        {
            _context = context;
            FirebaseAdminHelper.InitializeFirebase(); // Ensure Firebase is initialized
        }

        // GET: ManageUsers/Index
        public IActionResult Index()
        {
            var users = _context.Users
                .Select(u => new User
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    UserType = u.UserType,
                    IsActive = u.IsActive
                }).ToList();

            return View(users);
        }

        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int userId, string username, string email, bool isActive, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return RedirectToAction("Index", "ManageUsers", new { message = "User not found." });
            }

            // Update Firebase if email or password has changed
            try
            {
                if (user.Email != email || !string.IsNullOrEmpty(password))
                {
                    // Get Firebase User by Email
                    var firebaseUser = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(user.Email);
                    var updateRequest = new UserRecordArgs { Uid = firebaseUser.Uid };

                    if (user.Email != email)
                    {
                        updateRequest.Email = email;
                        user.Email = email;
                    }

                    if (!string.IsNullOrEmpty(password))
                    {
                        updateRequest.Password = password;
                        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                    }

                    await FirebaseAuth.DefaultInstance.UpdateUserAsync(updateRequest);
                }
            }
            catch (FirebaseAuthException ex)
            {
                TempData["ErrorMessage"] = $"Error updating Firebase: {ex.Message}";
                return RedirectToAction("Index", "ManageUsers");
            }

            // Update local database
            user.Username = username;
            user.IsActive = isActive;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ManageUsers", new { message = "Changes saved successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Toggle the user's active status
            user.IsActive = !user.IsActive;
            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        
        public async Task<IActionResult> CreateAdmin(User model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.PasswordHash))
                {
                    ModelState.AddModelError("PasswordHash", "Password is required.");
                    return View(model);
                }

                try
                {
                    // Check if the email is already registered in the local database
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Email == model.Email);

                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Email", "The email is already registered. Please use a different email.");
                        return View(model);
                    }

                    // Register user in Firebase
                    await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
                    {
                        Email = model.Email,
                        Password = model.PasswordHash,
                    });

                    // Hash password before saving to SSMS
                    model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);
                    model.UserType = "Admin"; // Ensure user type is set to Admin
                    model.IsActive = true;
                    model.DateCreated = DateTime.Now;

                    // Save the user in the local database
                    _context.Users.Add(model);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Admin account created successfully.";
                    return RedirectToAction("Index");
                }
                catch (FirebaseAuthException ex)
                {
                    TempData["ErrorMessage"] = $"Firebase error: {ex.Message}";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                }
            }

            return View(model); // If something goes wrong, return to the view with the model state
        }

    }
}
