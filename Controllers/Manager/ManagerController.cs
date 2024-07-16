using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bao.Controllers.Manager
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult mLanding()
        {
            return View();
        }
        public IActionResult ManageStaff() 
        {
            return View();
        }

    }
}
