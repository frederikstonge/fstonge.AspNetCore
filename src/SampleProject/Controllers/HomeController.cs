using Microsoft.AspNetCore.Mvc;

namespace SampleProject.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IUrlHelper urlHelper)
        {
            var url = urlHelper.Action("Index", "Home");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}