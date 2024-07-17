using Bao.Areas.Identity.Data;
using Bao.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bao.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly BaoContext _context;

        public AdminController(BaoContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult aLanding()
        {
            return View();
        }

        public IActionResult adminDashboard()
        {
            return View();
        }

        // API to get order data for the past three days
        [HttpGet]
        [Route("api/orders/last-three-days")]
        public async Task<IActionResult> GetOrdersForLastThreeDays()
        {
            var fromDate = DateTime.Now.AddDays(-3);

            var orders = await _context.Orders
                .Where(o => o.OrderDate >= fromDate)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalAmount = g.Sum(o => o.TotalAmount)
                })
                .ToListAsync();

            return Json(orders);
        }
    }
}
