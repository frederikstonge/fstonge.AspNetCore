using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace MvcApp.Translation
{
    public class CustomEndpointRoutingUrlHelper : UrlHelperBase
    {
        private readonly LinkGenerator _linkGenerator;

        public CustomEndpointRoutingUrlHelper(
            ActionContext actionContext,
            LinkGenerator linkGenerator)
            : base(actionContext)
        {
            _linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
        }

        public override string Action(UrlActionContext urlActionContext)
        {
            if (urlActionContext == null)
            {
                throw new ArgumentNullException(nameof(urlActionContext));
            }

            var values = GetValuesDictionary(urlActionContext.Values);

            var currentCulture = CultureInfo.CurrentCulture;
            if (AmbientValues.TryGetValue("culture", out var ambiantCulture))
            {
                currentCulture = new CultureInfo((string)ambiantCulture);
            }

            if (values.TryGetValue("culture", out var culture))
            {
                currentCulture = new CultureInfo((string)culture);
            }
            
            if (urlActionContext.Action != null)
            {
                var actionString = urlActionContext.Action;
                values["action"] = Translation.Action.ResourceManager.GetString(actionString.ToLower(), currentCulture);
            }
            else if (!values.ContainsKey("action") && AmbientValues.TryGetValue("action", out var action))
            {
                var actionString = (string) action;
                values["action"] = Translation.Action.ResourceManager.GetString(actionString.ToLower(), currentCulture);
            }
            
            if (urlActionContext.Controller != null)
            {
                var controllerString = urlActionContext.Controller;
                values["controller"] = Translation.Controller.ResourceManager.GetString(controllerString.ToLower(), currentCulture);
            }
            else if (!values.ContainsKey("controller") && AmbientValues.TryGetValue("controller", out var controller))
            {
                var controllerString = (string)controller;
                values["controller"] = Translation.Controller.ResourceManager.GetString(controllerString.ToLower(), currentCulture);
            }

            var path = _linkGenerator.GetPathByRouteValues(
                ActionContext.HttpContext,
                routeName: null,
                values,
                fragment: new FragmentString(urlActionContext.Fragment == null
                    ? null
                    : "#" + urlActionContext.Fragment));

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
                fragment: new FragmentString(routeContext.Fragment == null ? null : "#" + routeContext.Fragment));

            return GenerateUrl(routeContext.Protocol, routeContext.Host, path);
        }
    }
}