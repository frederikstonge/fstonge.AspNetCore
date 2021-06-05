using Microsoft.AspNetCore.Mvc;
using AspNetCore.Routing.Translation.Attributes;

namespace SampleProject.Controllers
{
    [Translate("en", "orders")]
    [Translate("fr", "commandes")]
    public class OrdersController : Controller
    {
        [Translate("fr", "liste")]
        public IActionResult List()
        {
            return View();
        }
    }
}
