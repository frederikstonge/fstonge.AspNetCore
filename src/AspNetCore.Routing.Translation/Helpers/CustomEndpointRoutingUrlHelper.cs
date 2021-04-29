using System;
using System.Globalization;
using System.Linq;
using AspNetCore.Routing.Translation.Models;
using AspNetCore.Routing.Translation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.Routing.Translation.Helpers
{
    internal class CustomEndpointRoutingUrlHelper : UrlHelperBase
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IRouteService _routeService;

        public CustomEndpointRoutingUrlHelper(
            ActionContext actionContext,
            LinkGenerator linkGenerator,
            IRouteService routeService)
            : base(actionContext)
        {
            _linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
            _routeService = routeService ?? throw new ArgumentNullException(nameof(routeService));
        }

        public override string Action(UrlActionContext urlActionContext)
        {
            if (urlActionContext == null)
            {
                throw new ArgumentNullException(nameof(urlActionContext));
            }

            var values = GetValuesDictionary(urlActionContext.Values);
            var currentCulture = GetCultureValue(values);

            var controllerValue = GetControllerValue(urlActionContext, values);
            if (!string.IsNullOrEmpty(controllerValue))
            {
                values[Constants.ControllerParameterName] = _routeService.GetControllerTranslatedValue(controllerValue, currentCulture);
            }

            var actionValue = GetActionValue(urlActionContext, values);
            if (!string.IsNullOrEmpty(actionValue))
            {
                values[Constants.ActionParameterName] = _routeService.GetActionTranslatedValue(controllerValue, actionValue, currentCulture);
            }

            if (urlActionContext.Controller == null && urlActionContext.Action == null)
            {
                if (AmbientValues.ContainsKey(Constants.IdParameterName) && 
                    AmbientValues.TryGetValue(Constants.IdParameterName, out var id))
                {
                    values[Constants.IdParameterName] = id;
                }
            }

            string path;
            
            var rules = _routeService.RouteRules.Where(r =>
                r.ControllerName.Equals(controllerValue, StringComparison.OrdinalIgnoreCase)).ToList();
            
            var rule = rules.FirstOrDefault(r => r.ActionName.Equals(actionValue, StringComparison.OrdinalIgnoreCase));
            if (rule == null)
            {
                rule = rules.FirstOrDefault(r => r.ActionName == null);
            }

            var fragment = urlActionContext.Fragment == null
                ? FragmentString.Empty
                : new FragmentString("#" + urlActionContext.Fragment);
            
            if (rule != null)
            {
                path = rule.GenerateUrlPathCallback(
                    currentCulture,
                    _routeService.GetControllerTranslatedValue(controllerValue, currentCulture),
                    _routeService.GetActionTranslatedValue(controllerValue,actionValue, currentCulture),
                    values,
                    fragment);
            }
            else
            {
                path = _linkGenerator.GetPathByRouteValues(
                    ActionContext.HttpContext,
                    routeName: null,
                    values,
                    fragment: fragment);
            }

            
            
            return GenerateUrl(urlActionContext.Protocol, urlActionContext.Host, path);
        }

        public override string RouteUrl(UrlRouteContext routeContext)
        {
            if (routeContext == null)
            {
                throw new ArgumentNullException(nameof(routeContext));
            }

            var path = _linkGenerator.GetPathByRouteValues(
                ActionContext.HttpContext,
                routeContext.RouteName,
                routeContext.Values,
                fragment: routeContext.Fragment == null ? FragmentString.Empty : new FragmentString("#" + routeContext.Fragment));

            return GenerateUrl(routeContext.Protocol, routeContext.Host, path);
        }

        private string GetCultureValue(RouteValueDictionary values)
        {
            var currentCulture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();
            if (AmbientValues.TryGetValue(Constants.CultureParameterName, out var ambiantCulture))
            {
                currentCulture = (string) ambiantCulture;
            }

            if (values.TryGetValue(Constants.CultureParameterName, out var culture))
            {
                currentCulture = (string)culture;
            }

            return currentCulture;
        }

        private string GetControllerValue(UrlActionContext urlActionContext, RouteValueDictionary values)
        {
            string controller = null;
            if (urlActionContext.Controller != null)
            {
                controller = urlActionContext.Controller;
            }
            else if (!values.ContainsKey(Constants.ControllerParameterName) && 
                     AmbientValues.TryGetValue(Constants.ControllerParameterName, out var controllerValue))
            {
                controller = (string)controllerValue;
            }

            return controller?.ToLower();
        }
        
        private string GetActionValue(UrlActionContext urlActionContext, RouteValueDictionary values)
        {
            string action = null;
            if (urlActionContext.Action != null)
            {
                action = urlActionContext.Action;
            }
            else if (!values.ContainsKey(Constants.ActionParameterName) && 
                     AmbientValues.TryGetValue(Constants.ActionParameterName, out var actionValue))
            {
                action = (string)actionValue;
            }

            return action?.ToLower();
        }
    }
}