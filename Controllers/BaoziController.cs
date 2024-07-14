using Microsoft.AspNetCore.Mvc;

namespace Bao.Controllers
{
    public class BaoziController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult bLanding()
        {
            return View();
        }

        public IActionResult ProdMenu()
        {
            return View();
        }
    }
}
