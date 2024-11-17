using JobPortal1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class PaymentController : Controller
    {
    private readonly JobPortalTester1Context _context;

    public PaymentController(JobPortalTester1Context context)
        {
        _context = context;
        }

    // GET: Payment/StartPayment
    public async Task<IActionResult> StartPayment(int interestId)
        {
        var interest = await _context.Interests.FindAsync(interestId);

        if (interest == null)
            {
            return NotFound("Interest not found");
            }

        ViewBag.InterestId = interestId;
        ViewBag.JobSeekerName = await _context.JobSeekers
            .Where(js => js.JobSeekerId == interest.JobSeekerId)
            .Select(js => js.FirstName + " " + js.LastName)
            .FirstOrDefaultAsync();

        return View(interest);
        }

    // POST: Payment/CompletePayment
    [HttpPost]
    public async Task<IActionResult> CompletePayment(int interestId, decimal amount, string paymentMethod)
        {
        var interest = await _context.Interests.FindAsync(interestId);
        if (interest == null) return NotFound("Interest not found");

        var payment = new Payment
            {
            InterestId = interestId,
            PaymentAmount = amount,
            PaymentDate = DateTime.Now,
            PaymentMethod = paymentMethod,
            PaymentStatus = "Success"
            };

        _context.Payments.Add(payment);
        interest.PaymentStatus = "Success";
        await _context.SaveChangesAsync();

        var notification = new Notification
            {
            EmployerId = interest.EmployerId,
            JobSeekerId = interest.JobSeekerId,
            NotificationType = "Payment",
            Message = "Employer has paid to view job seeker's information",
            CreatedAt = DateTime.Now,
            IsRead = false,
            IsActionRequired = false
            };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return RedirectToAction("PaymentSuccess", new { interestId });
        }

    // GET: Payment/PaymentSuccess
    public async Task<IActionResult> PaymentSuccess(int interestId)
        {
        var interest = await _context.Interests.FindAsync(interestId);
        if (interest == null) return NotFound("Interest not found");

        ViewBag.InterestId = interestId;
        return View(interest);
        }
    }