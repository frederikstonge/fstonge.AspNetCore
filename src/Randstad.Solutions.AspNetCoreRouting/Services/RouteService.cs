using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Randstad.Solutions.AspNetCoreRouting.Attributes;
using Randstad.Solutions.AspNetCoreRouting.Models;

namespace Randstad.Solutions.AspNetCoreRouting.Services
{
    internal class RouteService : IRouteService
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public RouteService(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }
        
        public List<TranslationRouteRule> RouteRules { get; } = new List<TranslationRouteRule>();

        public string GetControllerTranslatedValue(string controllerName, string culture)
        {
            var controllers = GetTranslatedControllers();
            var controller = controllers.Keys.FirstOrDefault(k =>
                k.Equals(controllerName, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(controller))
            {
                var attributes = controllers[controller];
                var attribute =
                    attributes.FirstOrDefault(a => a.Culture.Equals(culture, StringComparison.OrdinalIgnoreCase));

                if (attribute != null)
                {
                    return attribute.Value;
                }
            }
            
            return controllerName;
        }

        public string GetControllerName(string translatedName, string currentCulture)
        {
            var controllers = GetTranslatedControllers();
            if (controllers.Any(c => c.Value.Any(a =>
                a.Value.Equals(translatedName, StringComparison.OrdinalIgnoreCase) &&
                a.Culture.Equals(currentCulture, StringComparison.OrdinalIgnoreCase))))
            {
                var controller = controllers.FirstOrDefault(c => c.Value.Any(a =>
                    a.Value.Equals(translatedName, StringComparison.OrdinalIgnoreCase) &&
                    a.Culture.Equals(currentCulture, StringComparison.OrdinalIgnoreCase)));

                return controller.Key;
            }

            return translatedName;
        }

        public string GetActionTranslatedValue(string controllerName, string actionName, string culture)
        {
            var actions = GetTranslatedActions();
            var action = actions.Keys.FirstOrDefault(k =>
                k.Equals($"{controllerName}/{actionName}", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(action))
            {
                var attributes = actions[action];
                var attribute =
                    attributes.FirstOrDefault(a => a.Culture.Equals(culture, StringComparison.OrdinalIgnoreCase));

                if (attribute != null)
                {
                    return attribute.Value;
                }
            }
            
            return actionName;
        }

        public string GetActionName(string controllerName, string translatedName, string currentCulture)
        {
            var actions = GetTranslatedActions();
            if (actions.Any(c => c.Value.Any(a =>
                a.Value.Equals(translatedName, StringComparison.OrdinalIgnoreCase) &&
                a.Culture.Equals(currentCulture, StringComparison.OrdinalIgnoreCase))))
            {
                var prefix = $"{controllerName}/";
                var action = actions.FirstOrDefault(c => c.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                                                         c.Value.Any(a =>
                                                             a.Value.Equals(translatedName, StringComparison.OrdinalIgnoreCase) &&
                                                             a.Culture.Equals(currentCulture, StringComparison.OrdinalIgnoreCase)));
                
                return action.Key.Substring(prefix.Length, action.Key.Length - prefix.Length);
            }

            return translatedName;
        }

        private Dictionary<string, IEnumerable<TranslateAttribute>> GetTranslatedControllers()
        {
            var dictionary = new Dictionary<string, IEnumerable<TranslateAttribute>>();
            
            var items = _actionDescriptorCollectionProvider.ActionDescriptors.Items
                .OfType<ControllerActionDescriptor>()
                .Where(c => c.ControllerTypeInfo.GetCustomAttributes(typeof(TranslateAttribute), true).Any()).ToList();
            
            foreach (var item in items)
            {
                dictionary.TryAdd(
                    item.ControllerName,
                    item.ControllerTypeInfo.GetCustomAttributes(typeof(TranslateAttribute), true)
                        .OfType<TranslateAttribute>());
            }

            return dictionary;
        }
        
        private Dictionary<string, IEnumerable<TranslateAttribute>> GetTranslatedActions()
        {
            var dictionary = new Dictionary<string, IEnumerable<TranslateAttribute>>();
            
            var items = _actionDescriptorCollectionProvider.ActionDescriptors.Items
                .OfType<ControllerActionDescriptor>()
                .Where(c => c.MethodInfo.GetCustomAttributes(typeof(TranslateAttribute), true).Any()).ToList();
            
            foreach (var item in items)
            {
                dictionary.TryAdd(
                    $"{item.ControllerName}/{item.ActionName}",
                    item.MethodInfo.GetCustomAttributes(typeof(TranslateAttribute), true)
                        .OfType<TranslateAttribute>());
            }

            return dictionary;
        }
    }
}