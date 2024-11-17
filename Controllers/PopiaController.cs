using Microsoft.AspNetCore.Mvc;

namespace JobPortal1.Controllers
{
    public class PopiaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
