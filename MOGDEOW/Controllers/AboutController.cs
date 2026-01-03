using Microsoft.AspNetCore.Mvc;
using MOGDEOW.Attributes;

namespace MOGDEOW.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
