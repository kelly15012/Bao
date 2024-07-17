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

            var cartItem = await _context.Carts.Include(c => c.Product).FirstOrDefaultAsync(c => c.CartId == id && c.UserId == user.Id);
            if (cartItem == null)
            {
                return NotFound();
            }

            if (cartItem.Product == null || cartItem.Product.Quantity < quantity)
            {
                ModelState.AddModelError("", "Insufficient product quantity available.");
                return RedirectToAction("Cart");
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

        [HttpPost]
        public async Task<IActionResult> Checkout(List<int> selectedItems)
        {
            if (selectedItems == null || !selectedItems.Any())
            {
                return RedirectToAction("Cart", "Baozi"); // Redirect back to cart if no items are selected
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Cart", "Baozi");

            var cartItems = await _context.Carts
                                          .Include(c => c.Product)
                                          .Where(c => selectedItems.Contains(c.CartId) && c.UserId == user.Id)
                                          .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction("Cart", "Baozi");
            }

            // Validate product quantities before creating the order
            foreach (var item in cartItems)
            {
                if (item.Product == null || item.Product.Quantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"Insufficient quantity for product: {item.Product?.ProductName}");
                    return RedirectToAction("Cart");
                }
            }

            // Create a new order
            var order = new Order
            {
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(c => c.Product?.Price * c.Quantity ?? 0),
                Status = "Pending",
                UserId = user.Id,
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Product?.Price ?? 0
                }).ToList()
            };

            // Update product quantities
            foreach (var item in cartItems)
            {
                if (item.Product != null)
                {
                    item.Product.Quantity -= item.Quantity;
                }
            }

            _context.Orders.Add(order);
            _context.Carts.RemoveRange(cartItems); // Remove items from cart after placing the order
            await _context.SaveChangesAsync();

            return RedirectToAction("Payment", new { orderId = order.OrderId });
        }

        public async Task<IActionResult> Payment(int orderId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Cart", "Baozi");

            var order = await _context.Orders
                                      .Include(o => o.OrderItems!)
                                      .ThenInclude(oi => oi.Product!)
                                      .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View("~/Views/Baozi/Payment.cshtml", order);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(int orderId, string cardNumber, string expiryDate, string cvv, string paypalEmail, string bankingMethod)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Cart", "Baozi");

            var order = await _context.Orders
                                      .Include(o => o.OrderItems!)
                                      .ThenInclude(oi => oi.Product!)
                                      .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);


            if (order == null)
            {
                return NotFound();
            }

            order.Status = "Paid";

            // Here you would normally process the payment using a payment gateway API.
            // For the purpose of this example, we'll assume the payment is successful.

            if (order.OrderItems == null)
            {
                return RedirectToAction("OrderConfirmation", new { orderId = orderId });
            }

            // Update the product quantities
            foreach (var item in order.OrderItems)
            {
                var product = item.Product;
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                    if (product.Quantity <= 0)
                    {
                        product.Quantity = 0;
                        product.Status = false;
                    }
                }
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderConfirmation", new { orderId = orderId });
        }

        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Cart", "Baozi");

            var order = await _context.Orders
                                      .Include(o => o.OrderItems!)
                                      .ThenInclude(oi => oi.Product!)
                                      .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View("~/Views/Baozi/OrderConfirmation.cshtml", order);
        }
    }
}