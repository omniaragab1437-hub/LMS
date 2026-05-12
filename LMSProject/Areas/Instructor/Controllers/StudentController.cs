using Microsoft.AspNetCore.Mvc;

namespace LMSProject.Areas.Instructor.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
