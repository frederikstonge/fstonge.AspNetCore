using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using SoneparCanada.OpenCatalog.AspNetCoreRouting.Models;
using SoneparCanada.OpenCatalog.AspNetCoreRouting.Services;

namespace SoneparCanada.OpenCatalog.AspNetCoreRouting.Transformers
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
            if (!values.ContainsKey(Constants.ControllerParameterName) || !values.ContainsKey(Constants.ActionParameterName))
            {
                return new ValueTask<RouteValueDictionary>(Task.FromResult(values));
            }
            
            if (!values.ContainsKey(Constants.CultureParameterName))
            {
                var rqf = httpContext.Request.HttpContext.Features.Get<IRequestCultureFeature>();
                values[Constants.CultureParameterName] = rqf.RequestCulture.Culture.TwoLetterISOLanguageName.ToLower();
            }

            var culture = (string)values[Constants.CultureParameterName];
            var controller = (string) values[Constants.ControllerParameterName];
            var controllerName = _routeService.GetControllerName(controller, culture);
            values[Constants.ControllerParameterName] = controllerName;

            var action = (string) values[Constants.ActionParameterName];
            values[Constants.ActionParameterName] = _routeService.GetActionName(controllerName, action, culture);

                return new ValueTask<RouteValueDictionary>(Task.FromResult(values));
        }
    }
}
