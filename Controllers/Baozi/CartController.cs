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
    [Authorize(Roles = "Baozi")]
    public class CartController : Controller
    {
        private readonly BaoContext _context;
        private readonly UserManager<BaoUser> _userManager;

        public CartController(BaoContext context, UserManager<BaoUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cart
        public async Task<IActionResult> Cart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var userId = user.Id;
            var cartItems = await _context.Carts.Include(c => c.Product).Where(c => c.UserId == userId).ToListAsync();

            return View("/Views/Baozi/Cart.cshtml", cartItems);
        }

        // POST: Cart/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(int id, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == id && c.UserId == user.Id);
            if (cartItem == null)
            {
                return NotFound();
            }

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            return RedirectToAction("Cart");
        }

        // POST: Cart/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == id && c.UserId == user.Id);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Cart");
        }
    }
}

