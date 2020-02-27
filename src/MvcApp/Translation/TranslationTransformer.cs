using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace MvcApp.Translation
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
                var cultureInfo = new CultureInfo(culture);

                var controller = (string) values["controller"];
                values["controller"] = Controller.ResourceManager.GetKeyByValue(controller.ToLower(), cultureInfo);

                var action = (string) values["action"];
                values["action"] = Action.ResourceManager.GetKeyByValue(action.ToLower(), cultureInfo);
            }

            return new ValueTask<RouteValueDictionary>(Task.FromResult(values));;
        }
    }
}
