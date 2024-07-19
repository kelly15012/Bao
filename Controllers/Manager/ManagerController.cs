using Bao.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bao.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bao.Controllers.Manager
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //Get the data from database and pass to view
        [HttpGet]
        public async Task<IActionResult> ContactUsManagement()
        {
            var contacts = await _context.Contacts.ToListAsync();
            return View(contacts);
        }

        //Change status
        [HttpPost]
        public async Task<IActionResult> MarkAsResolved(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null && !contact.Status)
            {
                contact.Status = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ContactUsManagement));
        }

        private readonly BaoContext _context;

        public ManagerController(BaoContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> mLanding()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalOrdersToday = await _context.Orders
                .Where(o => o.OrderDate.Date == DateTime.Today)
                .CountAsync();

            var Model = new Dashboard
            {
                TotalUsers = totalUsers,
                TotalOrdersToday = totalOrdersToday
            };
            ViewBag.Title = "Manager Landing Page";
            return View(Model);
        }

        
    }
}
