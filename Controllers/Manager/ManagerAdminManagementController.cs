using Bao.Areas.Identity.Data;
using Bao.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bao.Controllers.Manager
{
    public class ManagerAdminManagementController : Controller
    {
        private readonly BaoContext _context;
        private readonly UserManager<BaoUser> _userManager;

        public ManagerAdminManagementController(BaoContext context, UserManager<BaoUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ManagerAdminManagement/AdminIndex
        public async Task<IActionResult> AdminIndex()
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (role == null)
            {
                return NotFound("Role Admin not found.");
            }

            var usersInRole = await _context.UserRoles
                .Where(ur => ur.RoleId == role.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            var users = await _context.Users
                .Where(u => usersInRole.Contains(u.Id))
                .ToListAsync();

            return View("~/Views/Manager/adminIndex.cshtml", users);
        }

        // GET: ManagerAdminManagement/Edit
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

            return View("~/Views/Manager/adminEdit.cshtml", user);
        }

        // POST: ManagerAdminManagement/Edit
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
                return RedirectToAction(nameof(AdminIndex));
            }
            return View("~/Views/Manager/adminEdit.cshtml", user);
        }

        // GET: ManagerAdminManagement/Delete
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

            return View("~/Views/Manager/adminDelete.cshtml", user);
        }

        // POST: ManagerAdminManagement/Delete
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
            return RedirectToAction(nameof(AdminIndex));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
