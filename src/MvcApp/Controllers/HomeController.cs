using Microsoft.AspNetCore.Mvc;
using SoneparCanada.OpenCatalog.AspNetCoreRouting.Attributes;

namespace MvcApp.Controllers
{
    [Translate("en", "home")]
    [Translate("fr", "accueil")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}