using Bao.Areas.Identity.Data;
using Bao.Data;
using Bao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bao.Controllers.Admin
{
    public class OrderManagementController : Controller
    {
        private readonly BaoContext _context;

        public OrderManagementController(BaoContext context)
        {
            _context = context;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> OrderIndex()
        {
            var orders = await _context.Orders
                                       .Include(o => o.OrderItems!)
                                       .ThenInclude(oi => oi.Product)
                                       .ToListAsync();

            return View("~/Views/Admin/OrderIndex.cshtml", orders);
        }

        // GET: Admin/Orders/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/EditOrder.cshtml", order);
        }

        // POST: Admin/Orders/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,Status")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var orderInDb = await _context.Orders.FindAsync(id);
                    if (orderInDb == null)
                    {
                        return NotFound();
                    }

                    orderInDb.Status = order.Status;

                    _context.Update(orderInDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(OrderIndex));
            }
            return View("~/Views/Admin/EditOrder.cshtml", order);
        }

        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/DeleteOrder.cshtml", order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(OrderIndex));
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                                      .Where(o => o.OrderId == id)
                                      .Include(o => o.OrderItems!)
                                      .ThenInclude(oi => oi.Product)
                                      .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/OrderDetails.cshtml", order);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
