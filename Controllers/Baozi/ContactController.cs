using Bao.Areas.Identity.Data;
using Bao.Data;
using Bao.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bao.Controllers
{
    public class BaoziController : Controller
    {
        private readonly BaoContext _context;
        private readonly UserManager<BaoUser> _userManager;

        public BaoziController(BaoContext context, UserManager<BaoUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ContactUs(Contact contact)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    contact.UserId = user.Id;
                    _context.Contacts.Add(contact);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Your message has been sent successfully.";
                    return RedirectToAction("ContactUs");
                }
            }
            TempData["ErrorMessage"] = "There was an error. Please try again.";
            return View(contact);
        }
    }
}
