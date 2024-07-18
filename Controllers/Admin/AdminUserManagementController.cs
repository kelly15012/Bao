using Bao.Areas.Identity.Data;
using Bao.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bao.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserManagementController : Controller
    {
        private readonly BaoContext _context;
        private readonly UserManager<BaoUser> _userManager;

        public AdminUserManagementController(BaoContext context, UserManager<BaoUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: AdminUserManagement/UserIndex
        public async Task<IActionResult> UserIndex()
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Baozi");
            if (role == null)
            {
                return NotFound("Role Baozi not found.");
            }

            var usersInRole = await _context.UserRoles
                .Where(ur => ur.RoleId == role.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            var users = await _context.Users
                .Where(u => usersInRole.Contains(u.Id))
                .ToListAsync();

            return View("~/Views/Admin/userIndex.cshtml", users);
        }

        // GET: AdminUserManagement/Edit
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/userEdit.cshtml", user);
        }

        // POST: AdminUserManagement/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,Gender,DateOfBirth")] BaoUser user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userInDb = await _context.Users.FindAsync(id);
                    if (userInDb == null)
                    {
                        return NotFound();
                    }

                    userInDb.FirstName = user.FirstName;
                    userInDb.LastName = user.LastName;
                    userInDb.Gender = user.Gender;
                    userInDb.DateOfBirth = user.DateOfBirth;

                    _context.Update(userInDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(UserIndex));
            }
            return View("~/Views/Admin/userEdit.cshtml", user);
        }

        // GET: AdminUserManagement/Delete
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/userDelete.cshtml", user);
        }

        // POST: AdminUserManagement/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(UserIndex));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
