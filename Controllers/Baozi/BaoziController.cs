using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bao.Data;
using Bao.Models;

namespace Bao.Controllers.Baozi
{
    [Authorize(Roles = "Baozi")]
    public class BaoziController : Controller
    {
        private readonly BaoContext _context;

        public BaoziController(BaoContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult bLanding()
        {
            return View();
        }

        public IActionResult ProdMenu()
        {
            return View();
        }

        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }

        public IActionResult Order()
        {
            return View();
        }

        public IActionResult OrderDetails()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs(Contact contact)
        {
            if (ModelState.IsValid)
            {

                _context.Add(contact);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

        }

        public IActionResult FAQ()
        {
            return View();
        }
    }
}
