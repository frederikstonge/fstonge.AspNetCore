using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace SoneparCanada.OpenCatalog.AspNetCoreRouting.Controllers
{
    public sealed class RedirectController : Controller
    {
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var culture = rqf.RequestCulture.Culture.TwoLetterISOLanguageName;
            return Redirect($"/{culture}/");
        }
    }
}