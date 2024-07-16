using Bao.Data;
using Bao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace Bao.Controllers.Baozi
{
    [Authorize(Roles = "Baozi")]
    public class BaoziController : Controller
    {
        private readonly BaoContext _context;
        public BaoziController(BaoContext context)
        {
            _context = context;
        }

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
