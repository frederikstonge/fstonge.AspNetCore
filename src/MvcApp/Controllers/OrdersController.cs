using Microsoft.AspNetCore.Mvc;
using Randstad.Solutions.AspNetCoreRouting.Attributes;

namespace MvcApp.Controllers
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
