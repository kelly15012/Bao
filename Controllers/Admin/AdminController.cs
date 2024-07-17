using Bao.Data;
using Bao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bao.Controllers.Admin
{
    [Route("Admin/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly BaoContext _context;

        public AdminController(BaoContext context)
        {
            _context = context;
        }

        [HttpGet("last-three-days")]
        public async Task<IActionResult> GetOrdersForLastThreeDays()
        {
            var threeDaysAgo = DateTime.Now.AddDays(-3);
            var orders = await _context.Orders
                                       .Where(o => o.OrderDate >= threeDaysAgo)
                                       .GroupBy(o => o.OrderDate.Date)
                                       .Select(g => new
                                       {
                                           Date = g.Key,
                                           TotalAmount = g.Sum(o => o.TotalAmount)
                                       })
                                       .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("category-sales")]
        public async Task<IActionResult> GetCategorySales()
        {
            var categorySales = await _context.OrderItems
                                              .Include(oi => oi.Product)
                                              .ThenInclude(p => p.Category)
                                              .GroupBy(oi => oi.Product!.Category!.categoryName)
                                              .Select(g => new
                                              {
                                                  Category = g.Key,
                                                  TotalSales = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                                              })
                                              .ToListAsync();

            return Ok(categorySales);
        }

        [HttpGet("dashboard")]
        public IActionResult AdminDashboard()
        {
            return View("~/Views/Admin/adminDashboard.cshtml");
        }

        [HttpGet("landing")]
        public IActionResult ALanding()
        {
            return View("~/Views/Admin/aLanding.cshtml");
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            return View("~/Views/Admin/Index.cshtml");
        }
    }
}
