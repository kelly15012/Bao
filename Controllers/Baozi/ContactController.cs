using Microsoft.AspNetCore.Mvc;
using Bao.Models;

namespace Bao.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Contact model)
        {
            if (ModelState.IsValid)
            {
                // Handle form submission (e.g., save data, send email, etc.)
                TempData["Message"] = "Thank you for contacting us. We will get back to you soon.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}
