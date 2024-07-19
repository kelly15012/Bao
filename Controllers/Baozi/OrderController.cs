using Bao.Areas.Identity.Data;
using Bao.Data;
using Bao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bao.Controllers.Baozi
{
    public class OrderController : Controller
    {
        private readonly BaoContext _context;
        private readonly UserManager<BaoUser> _userManager;

        public OrderController(BaoContext context, UserManager<BaoUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var userId = user.Id;
            var orders = await _context.Orders
                                       .Where(o => o.UserId == userId)
                                       .Include(o => o.OrderItems!)
                                       .ThenInclude(oi => oi.Product)
                                       .ToListAsync();

            return View("~/Views/Baozi/Order.cshtml", orders);
        }

        // GET: Order/Details
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var userId = user.Id;
            var order = await _context.Orders
                                      .Where(o => o.OrderId == id && o.UserId == userId)
                                      .Include(o => o.OrderItems!)
                                      .ThenInclude(oi => oi.Product)
                                      .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return View("~/Views/Baozi/OrderDetails.cshtml", order);
        }
    }
}