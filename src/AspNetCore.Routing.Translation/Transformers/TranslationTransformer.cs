using System.Threading.Tasks;
using AspNetCore.Routing.Translation.Models;
using AspNetCore.Routing.Translation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.Routing.Translation.Transformers
{
    public class TranslationTransformer : DynamicRouteValueTransformer
    {
        private readonly IRouteService _routeService;
        public TranslationTransformer(IRouteService routeService)
        {
            _routeService = routeService;
        }
        
        public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            if (!values.ContainsKey(RouteValue.Controller) || !values.ContainsKey(RouteValue.Action))
            {
                return new ValueTask<RouteValueDictionary>(Task.FromResult(values));
            }

            var culture = (string)values[RouteValue.Culture];
            
            var controller = (string) values[RouteValue.Controller];
            var controllerName = _routeService.GetControllerName(controller, culture);
            values[RouteValue.Controller] = controllerName;

            var action = (string) values[RouteValue.Action];
            values[RouteValue.Action] = _routeService.GetActionName(controllerName, action, culture);
            return new ValueTask<RouteValueDictionary>(Task.FromResult(values));
        }
    }
}
