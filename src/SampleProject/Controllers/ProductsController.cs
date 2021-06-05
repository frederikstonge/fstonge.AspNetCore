using Microsoft.AspNetCore.Mvc;
using AspNetCore.Routing.Translation.Attributes;

namespace SampleProject.Controllers
{
    [Translate("fr", "produits")]
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
