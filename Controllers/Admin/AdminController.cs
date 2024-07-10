using Microsoft.AspNetCore.Mvc;

namespace Bao.Controllers.Admin
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult aLanding()
        {
            return View();
        }

        
    }
}
