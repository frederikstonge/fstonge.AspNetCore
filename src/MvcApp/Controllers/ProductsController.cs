using Microsoft.AspNetCore.Mvc;

namespace MvcApp.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index(string id)
        {
            return View();
        }
        
        public IActionResult Detail(string id)
        {
            return View();
        }
    }
}
