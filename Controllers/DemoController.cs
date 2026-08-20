namespace DemoMVC1.Controllers
{

    using Microsoft.AspNetCore.Mvc;

    public class DemoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


    }
}