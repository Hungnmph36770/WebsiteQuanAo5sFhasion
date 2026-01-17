using Microsoft.AspNetCore.Mvc;
using Website.Areas.Admin.Attributes;


namespace Website.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Area("Admin")]
        [CheckLogin]
        public IActionResult Index()
        {
            return View();
        }
       }
    }

