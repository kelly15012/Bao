using Bao.Areas.Identity.Data;
using Bao.Data;
using Bao.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                    contact.Status = false;
                    _context.Contacts.Add(contact);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Your message has been sent successfully.";
                    return RedirectToAction("ContactUs");
                }
            }
            TempData["ErrorMessage"] = "There was an error. Please try again.";
            return View(contact);
        }

        // GET: Manager/FeedbackManagement
        public async Task<IActionResult> FeedbackIndex()
        {
            var feedbacks = await _context.Contacts
                                          .Include(c => c.User)
                                          .ToListAsync();
            return View("~/Views/Manager/ContactUsManagement.cshtml", feedbacks);
        }

        // POST: Manager/FeedbackManagement/MarkAsResolved
        [HttpPost]
        public async Task<IActionResult> MarkAsResolved(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            contact.Status = true;
            _context.Update(contact);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(FeedbackIndex));
        }


    }
}
