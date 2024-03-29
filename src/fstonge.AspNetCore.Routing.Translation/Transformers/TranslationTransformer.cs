using System.Threading.Tasks;
using fstonge.AspNetCore.Routing.Translation.Models;
using fstonge.AspNetCore.Routing.Translation.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace fstonge.AspNetCore.Routing.Translation.Transformers
{
    internal class TranslationTransformer : DynamicRouteValueTransformer
    {
        private readonly IRouteService _routeService;
        private readonly RequestLocalizationOptions _transOptions;

        public TranslationTransformer(IRouteService routeService, IOptions<RequestLocalizationOptions> transOptions)
        {
            _routeService = routeService;
            _transOptions = transOptions.Value;
        }

        public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            if (!values.ContainsKey(RouteValue.Controller) || !values.ContainsKey(RouteValue.Action))
            {
                return new ValueTask<RouteValueDictionary>(Task.FromResult(values));
            }

            var culture = values.ContainsKey(RouteValue.Culture)
                ? (string)values[RouteValue.Culture]
                : _transOptions.DefaultRequestCulture.Culture.ToString();

            var controller = (string)values[RouteValue.Controller];
            var controllerName = _routeService.GetControllerName(controller, culture);
            values[RouteValue.Controller] = controllerName;

            var action = (string)values[RouteValue.Action];
            var actionName = _routeService.GetActionName(controllerName, action, culture);
            values[RouteValue.Action] = actionName;
            return new ValueTask<RouteValueDictionary>(Task.FromResult(values));
        }
    }
}
