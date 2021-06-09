using Microsoft.AspNetCore.Mvc;
using AspNetCore.Routing.Translation.Attributes;

namespace SampleProject.Controllers
{
    [Translate("en-CA", "orders")]
    [Translate("en", "orders")]
    [Translate("fr-CA", "commandes")]
    [Translate("fr", "commandes")]
    public class OrdersController : Controller
    {
        public OrdersController(IUrlHelper urlHelper)
        {
            var url = urlHelper.Action("Index", "Home");
        }
        
        [Translate("fr-CA", "liste")]
        [Translate("fr", "liste")]
        public IActionResult List()
        {
            return View();
        }
    }
}
