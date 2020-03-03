using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Randstad.Solutions.AspNetCoreRouting.Helpers;
using Randstad.Solutions.AspNetCoreRouting.Services;

namespace Randstad.Solutions.AspNetCoreRouting.Transformers
{
    internal class TranslationTransformer : DynamicRouteValueTransformer
    {
        private readonly IRouteService _routeService;
        public TranslationTransformer(IRouteService routeService)
        {
            _routeService = routeService;
        }
        
        public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            if (!values.ContainsKey("controller") || !values.ContainsKey("action"))
            {
                return new ValueTask<RouteValueDictionary>(Task.FromResult(values));
            }
            
            if (!values.ContainsKey("culture"))
            {
                var rqf = httpContext.Request.HttpContext.Features.Get<IRequestCultureFeature>();
                values["culture"] = rqf.RequestCulture.Culture.TwoLetterISOLanguageName.ToLower();
            }

            var culture = (string)values["culture"];
            if (culture == "fr" || culture == "en")
            {
                var controller = (string) values["controller"];
                values["controller"] = _routeService.GetControllerNameFromTranslatedValue(controller, culture);

                var action = (string) values["action"];
                values["action"] = _routeService.GetActionNameFromTranslatedValue(action, culture);
            }

            return new ValueTask<RouteValueDictionary>(Task.FromResult(values));
        }
    }
}
