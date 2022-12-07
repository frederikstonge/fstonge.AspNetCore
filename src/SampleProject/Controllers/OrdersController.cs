using Microsoft.AspNetCore.Mvc;
using fstonge.AspNetCore.Routing.Translation.Attributes;

namespace SampleProject.Controllers
{
    [Translate("en-CA", "orders")]
    [Translate("en", "orders")]
    [Translate("fr-CA", "commandes-de-test")]
    [Translate("fr", "commandes-de-test")]
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
