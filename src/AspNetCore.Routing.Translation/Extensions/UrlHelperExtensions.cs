using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCore.Routing.Translation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace AspNetCore.Routing.Translation.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string Action(this IUrlHelper urlHelper, RouteInfo routeInfo, string fragment = null)
        {
            return urlHelper.Action(routeInfo.Action, routeInfo.Controller, routeInfo.RouteValues, null, null, fragment);
        }
        
        public static RouteInfo GetRouteInfo(this IUrlHelper urlHelper)
        {
            var httpContext = urlHelper.ActionContext.HttpContext;
            var controller = httpContext.GetRouteData().Values[RouteValue.Controller]?.ToString();
            var action = httpContext.GetRouteData().Values[RouteValue.Action]?.ToString();

            var routeValues = httpContext.Request.Query.ToList();

            var id = httpContext.GetRouteData().Values[RouteValue.Id]?.ToString();
            if (!string.IsNullOrEmpty(id))
            {
                routeValues.AddParameterValue(RouteValue.Id, id);
            }
            
            var culture = httpContext.GetRouteData().Values[RouteValue.Culture]?.ToString();
            if (!string.IsNullOrEmpty(culture))
            {
                routeValues.AddParameterValue(RouteValue.Culture, culture);
            }

            return new RouteInfo()
            {
                Controller = controller,
                Action = action,
                RouteValues = routeValues
            };
        }
        
        public static void AddParameterValue(
            this IList<KeyValuePair<string, StringValues>> elements,
            string key,
            string value)
        {
            if (elements != null && !string.IsNullOrEmpty(value))
            {
                if (elements.Any(e => e.Key.Equals(key, StringComparison.OrdinalIgnoreCase)))
                {
                    var element = elements.First(e => e.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                    elements.Remove(element);
                    var newElements = element.Value.ToList();
                    newElements.Add(value);
                    elements.Add(new KeyValuePair<string, StringValues>(key, new StringValues(newElements.ToArray())));
                }
                else
                {
                    elements.Add(new KeyValuePair<string, StringValues>(key, value));
                }
            }
        }

        public static void ChangeParameterValue(
            this IList<KeyValuePair<string, StringValues>> elements,
            string key,
            string value)
        {
            if (elements != null && !string.IsNullOrEmpty(value))
            {
                if (elements.Any(e => e.Key.Equals(key, StringComparison.OrdinalIgnoreCase)))
                {
                    var element = elements.First(e => e.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                    elements.Remove(element);
                    var newElements = new[] { value };
                    elements.Add(new KeyValuePair<string, StringValues>(key, new StringValues(newElements)));
                }
                else
                {
                    elements.Add(new KeyValuePair<string, StringValues>(key, value));
                }
            }
        }

        public static void RemoveParameterValue(
            this IList<KeyValuePair<string, StringValues>> elements,
            string key,
            string value)
        {
            if (elements != null && !string.IsNullOrEmpty(value))
            {
                if (elements.Any(e => e.Key.Equals(key, StringComparison.OrdinalIgnoreCase)))
                {
                    var element = elements.First(e => e.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                    elements.Remove(element);
                    var newElements = element.Value.Where(s => !s.Equals(value, StringComparison.OrdinalIgnoreCase))
                        .ToArray();
                    if (newElements.Any())
                    {
                        elements.Add(new KeyValuePair<string, StringValues>(key, new StringValues(newElements)));
                    }
                }
            }
        }

        public static void RemoveParameter(this IList<KeyValuePair<string, StringValues>> elements, string key)
        {
            if (elements != null)
            {
                if (elements.Any(e => e.Key.Equals(key, StringComparison.OrdinalIgnoreCase)))
                {
                    var element = elements.First(e => e.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                    elements.Remove(element);
                }
            }
        }
    }
}