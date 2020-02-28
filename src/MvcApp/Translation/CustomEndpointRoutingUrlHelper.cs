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
            var currentCulture = GetCurrentCulture(urlActionContext, values);

            var controller = GetControllerValue(urlActionContext, values);
            if (!string.IsNullOrEmpty(controller))
            {
                values["controller"] = Translation.Controller.ResourceManager.GetString(controller, currentCulture);
            }

            var action = GetActionValue(urlActionContext, values);
            if (!string.IsNullOrEmpty(action))
            {
                values["action"] = Translation.Action.ResourceManager.GetString(action, currentCulture);
            }

            string path;
            if (controller.Equals("products", StringComparison.OrdinalIgnoreCase))
            {
                path = string.Empty;
                var id = GetParameterValue(values, "id");
                var cultureString = currentCulture.TwoLetterISOLanguageName.ToLower();
                if (action.Equals("detail", StringComparison.OrdinalIgnoreCase))
                {
                    path = $"{cultureString}/{values["controller"]}/13-lightning/p-{id}-test-de-rewrite";
                }
                else if (action.Equals("index", StringComparison.OrdinalIgnoreCase))
                {
                    path = $"{cultureString}/{values["controller"]}/13-lightning";
                }
            }
            else
            {
                path = _linkGenerator.GetPathByRouteValues(
                    ActionContext.HttpContext,
                    routeName: null,
                    values,
                    fragment: new FragmentString(urlActionContext.Fragment == null
                        ? null
                        : "#" + urlActionContext.Fragment));
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
                fragment: new FragmentString(routeContext.Fragment == null ? null : "#" + routeContext.Fragment));

            return GenerateUrl(routeContext.Protocol, routeContext.Host, path);
        }

        private CultureInfo GetCurrentCulture(UrlActionContext urlActionContext, RouteValueDictionary values)
        {
            var currentCulture = CultureInfo.CurrentCulture;
            if (AmbientValues.TryGetValue("culture", out var ambiantCulture))
            {
                var ambientCultureString = (string) ambiantCulture;
                if (ambientCultureString == "fr" || ambientCultureString == "en")
                {
                    currentCulture = new CultureInfo(ambientCultureString);
                }
            }

            if (values.TryGetValue("culture", out var culture))
            {
                var cultureString = (string) culture;
                if (cultureString == "fr" || cultureString == "en")
                {
                    currentCulture = new CultureInfo(cultureString);
                }
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
            else if (!values.ContainsKey("controller") && AmbientValues.TryGetValue("controller", out var controllerValue))
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
            else if (!values.ContainsKey("action") && AmbientValues.TryGetValue("action", out var actionValue))
            {
                action = (string)actionValue;
            }

            return action?.ToLower();
        }

        private string GetParameterValue(RouteValueDictionary values, string parameterName)
        {
            string parameter = null;
            if (values.ContainsKey(parameterName) && values.TryGetValue(parameterName, out var parameterValue))
            {
                parameter = (string)parameterValue;
            }

            return parameter;
        }
    }
}