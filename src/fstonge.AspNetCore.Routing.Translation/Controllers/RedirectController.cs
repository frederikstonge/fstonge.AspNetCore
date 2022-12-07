using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace fstonge.AspNetCore.Routing.Translation.Controllers
{
    public sealed class RedirectController : Controller
    {
        private readonly RequestLocalizationOptions _options;

        public RedirectController(IOptions<RequestLocalizationOptions> options)
        {
            _options = options.Value;
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var culture = rqf?.RequestCulture.Culture ?? _options.DefaultRequestCulture.Culture;
            var currentCulture = _options.SupportedCultures?.FirstOrDefault(c => c.Equals(culture)) ?? culture;

            return Redirect($"/{currentCulture}/");
        }
    }
}