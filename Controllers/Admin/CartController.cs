using Microsoft.AspNetCore.Mvc;
using Bao.Models;
using Bao.Data;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bao.Controllers.Admin
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
