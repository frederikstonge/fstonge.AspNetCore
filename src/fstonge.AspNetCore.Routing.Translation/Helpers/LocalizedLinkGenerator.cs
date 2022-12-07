using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fstonge.AspNetCore.Routing.Translation.Models;
using fstonge.AspNetCore.Routing.Translation.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace fstonge.AspNetCore.Routing.Translation.Helpers
{
    public sealed class LocalizedLinkGenerator : LinkGenerator
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IRouteService _routeService;
        private readonly IOptions<RequestLocalizationOptions> _requestLocalizationOptions;
        private readonly List<ICustomTranslation> _customRoutes;

        public LocalizedLinkGenerator(
            LinkGenerator linkGenerator,
            IRouteService routeService,
            IOptions<RequestLocalizationOptions> requestLocalizationOptions,
            IEnumerable<ICustomTranslation> customRoutes)
        {
            _linkGenerator = linkGenerator;
            _routeService = routeService;
            _requestLocalizationOptions = requestLocalizationOptions;
            _customRoutes = customRoutes.ToList();
        }

        public override string GetPathByAddress<TAddress>(
            HttpContext httpContext,
            TAddress address,
            RouteValueDictionary values,
            RouteValueDictionary ambientValues = null,
            PathString? pathBase = null,
            FragmentString fragment = default,
            LinkOptions options = null)
        {
            var controllerName = (string)values[RouteValue.Controller];
            var actionName = (string)values[RouteValue.Action];
            RewriteValuesDictionary(values);
            var customUrl = GenerateCustomUrl(controllerName, actionName, values, fragment);
            if (!string.IsNullOrEmpty(customUrl))
            {
                return customUrl;
            }

            return _linkGenerator.GetPathByAddress(
                httpContext,
                address,
                values,
                ambientValues,
                pathBase,
                fragment,
                options);
        }

        public override string GetPathByAddress<TAddress>(
            TAddress address,
            RouteValueDictionary values,
            PathString pathBase = default,
            FragmentString fragment = default,
            LinkOptions options = null)
        {
            var controllerName = (string)values[RouteValue.Controller];
            var actionName = (string)values[RouteValue.Action];
            RewriteValuesDictionary(values);
            var customUrl = GenerateCustomUrl(controllerName, actionName, values, fragment);
            if (!string.IsNullOrEmpty(customUrl))
            {
                return customUrl;
            }

            return _linkGenerator.GetPathByAddress(address, values, pathBase, fragment, options);
        }

        public override string GetUriByAddress<TAddress>(
            HttpContext httpContext,
            TAddress address,
            RouteValueDictionary values,
            RouteValueDictionary ambientValues = null,
            string scheme = null,
            HostString? host = null,
            PathString? pathBase = null,
            FragmentString fragment = default,
            LinkOptions options = null)
        {
            var controllerName = (string)values[RouteValue.Controller];
            var actionName = (string)values[RouteValue.Action];
            RewriteValuesDictionary(values);
            var customUrl = GenerateCustomUrl(controllerName, actionName, values, fragment);
            if (!string.IsNullOrEmpty(customUrl))
            {
                return customUrl;
            }

            return _linkGenerator.GetUriByAddress(
                httpContext,
                address,
                values,
                ambientValues,
                scheme,
                host,
                pathBase,
                fragment,
                options);
        }

        public override string GetUriByAddress<TAddress>(
            TAddress address,
            RouteValueDictionary values,
            string scheme,
            HostString host,
            PathString pathBase = default,
            FragmentString fragment = default,
            LinkOptions options = null)
        {
            var controllerName = (string)values[RouteValue.Controller];
            var actionName = (string)values[RouteValue.Action];
            RewriteValuesDictionary(values);
            var customUrl = GenerateCustomUrl(controllerName, actionName, values, fragment);
            if (!string.IsNullOrEmpty(customUrl))
            {
                return customUrl;
            }

            return _linkGenerator.GetUriByAddress(address, values, scheme, host, pathBase, fragment, options);
        }

        private void RewriteValuesDictionary(RouteValueDictionary values)
        {
            var currentCulture = GetCultureValue(values);
            if (!string.IsNullOrEmpty(currentCulture) &&
                _requestLocalizationOptions.Value.SupportedCultures!.Count > 1)
            {
                values[RouteValue.Culture] = currentCulture;
            }

            var controllerValue = (string)values[RouteValue.Controller];
            if (!string.IsNullOrEmpty(controllerValue))
            {
                values[RouteValue.Controller] = _routeService.GetControllerTranslatedValue(
                    controllerValue,
                    currentCulture);
            }

            var actionValue = (string)values[RouteValue.Action];
            if (!string.IsNullOrEmpty(actionValue))
            {
                values[RouteValue.Action] = _routeService.GetActionTranslatedValue(
                controllerValue,
                actionValue,
                currentCulture);
            }
        }

        private string GetCultureValue(RouteValueDictionary values)
        {
            var currentCulture = CultureInfo.CurrentCulture.ToString();
            if (values.TryGetValue(RouteValue.Culture, out var culture))
            {
                currentCulture = (string)culture;
            }

            return currentCulture;
        }

        private string GenerateCustomUrl(
            string controllerName,
            string actionName,
            RouteValueDictionary values,
            FragmentString fragment = default)
        {
            var rules = _customRoutes.Where(r =>
                r.ControllerName.Equals(controllerName, StringComparison.OrdinalIgnoreCase)).ToList();

            var rule = rules.FirstOrDefault(r => r.ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase)) ??
                       rules.FirstOrDefault(r => r.ActionName == null);

            if (rule == null)
            {
                return null;
            }

            try
            {
                return rule.GenerateUrlPath(values, fragment);
            }
            catch
            {
                return null;
            }
        }
    }
}