using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bao.Controllers
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
    }
}
