using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bao.Controllers.Baozi
{
    [Authorize(Roles = "Baozi")]
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

        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult CheckOut(int id)
        {
            return View();
        }
    }
}
