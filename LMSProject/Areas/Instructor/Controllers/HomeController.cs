using Microsoft.AspNetCore.Mvc;

namespace LMSProject.Areas.Instructor.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
