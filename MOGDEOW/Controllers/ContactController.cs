using Microsoft.AspNetCore.Mvc;
using MOGDEOW.Attributes;

namespace MOGDEOW.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
