using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Randstad.Solutions.AspNetCoreRouting.Helpers;

namespace Randstad.Solutions.AspNetCoreRouting.Transformers
{
    public class TranslationTransformer : DynamicRouteValueTransformer
    {
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
                values["controller"] = TranslationAttributeHelper.GetControllerFromTranslatedValue(controller, culture);

                var action = (string) values["action"];
                values["action"] = TranslationAttributeHelper.GetActionFromTranslatedValue(action, culture);
            }

            return new ValueTask<RouteValueDictionary>(Task.FromResult(values));
        }
    }
}
