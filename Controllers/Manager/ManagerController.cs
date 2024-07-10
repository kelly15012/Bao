using Microsoft.AspNetCore.Mvc;

namespace Bao.Controllers.Manager
{
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
    }
}
