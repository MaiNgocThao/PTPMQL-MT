namespace DemoMVC1.Controllers
{

    using Microsoft.AspNetCore.Mvc;

    public class Lesson4Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Welcome()
        {
            ViewData["Message"] = "Your welcome message";

            return View();
        }
    }
}