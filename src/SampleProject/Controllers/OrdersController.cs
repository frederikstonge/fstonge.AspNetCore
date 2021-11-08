using Microsoft.AspNetCore.Mvc;
using AspNetCore.Routing.Translation.Attributes;
using AspNetCore.Routing.Translation.Helpers;
using Microsoft.AspNetCore.Routing;

namespace SampleProject.Controllers
{
    [Translate("en-CA", "orders")]
    [Translate("en", "orders")]
    [Translate("fr-CA", "commandes")]
    [Translate("fr", "commandes")]
    public class OrdersController : Controller
    {
        [Translate("fr-CA", "liste")]
        [Translate("fr", "liste")]
        public IActionResult List()
        {
            return View();
        }
    }
}
