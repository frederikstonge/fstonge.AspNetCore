using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCore.Routing.Translation.Attributes;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace AspNetCore.Routing.Translation.Services
{
    internal class RouteService : IRouteService
    {
        private readonly Dictionary<string, IEnumerable<TranslateAttribute>> _translatedControllers;
        private readonly Dictionary<string, IEnumerable<TranslateAttribute>> _translatedActions;

        public RouteService(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _translatedControllers = GetTranslatedControllers(actionDescriptorCollectionProvider);
            _translatedActions = GetTranslatedActions(actionDescriptorCollectionProvider);
        }

        public string GetControllerTranslatedValue(string controllerName, string culture)
        {
            var controller = _translatedControllers.Keys.FirstOrDefault(k =>
                k.Equals(controllerName, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(controller))
            {
                var attributes = _translatedControllers[controller];
                var attribute =
                    attributes.FirstOrDefault(a => a.Culture.Equals(culture, StringComparison.OrdinalIgnoreCase));

                if (attribute != null)
                {
                    return attribute.Value;
                }
            }
            
            return controllerName.ToLowerInvariant();
        }

        public string GetControllerName(string translatedName, string currentCulture)
        {
            if (_translatedControllers.Any(c => c.Value.Any(a =>
                a.Value.Equals(translatedName, StringComparison.OrdinalIgnoreCase) &&
                a.Culture.Equals(currentCulture, StringComparison.OrdinalIgnoreCase))))
            {
                var controller = _translatedControllers.FirstOrDefault(c => c.Value.Any(a =>
                    a.Value.Equals(translatedName, StringComparison.OrdinalIgnoreCase) &&
                    a.Culture.Equals(currentCulture, StringComparison.OrdinalIgnoreCase)));

                return controller.Key;
            }

            return translatedName.ToLowerInvariant();
        }

        public string GetActionTranslatedValue(string controllerName, string actionName, string culture)
        {
            var action = _translatedActions.Keys.FirstOrDefault(k =>
                k.Equals($"{controllerName}/{actionName}", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(action))
            {
                var attributes = _translatedActions[action];
                var attribute = attributes.FirstOrDefault(a => a.Culture.Equals(culture, StringComparison.OrdinalIgnoreCase));

                if (attribute != null)
                {
                    return attribute.Value;
                }
            }
            
            return actionName.ToLowerInvariant();
        }

        public string GetActionName(string controllerName, string translatedName, string currentCulture)
        {
            var prefix = $"{controllerName}/";
            if (_translatedActions.Any(c => c.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                                            c.Value.Any(a =>
                                                a.Value.Equals(translatedName, StringComparison.OrdinalIgnoreCase) &&
                                                a.Culture.Equals(currentCulture, StringComparison.OrdinalIgnoreCase))))
            {
                var action = _translatedActions.FirstOrDefault(c => c.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                                                                    c.Value.Any(a =>
                                                                        a.Value.Equals(translatedName, StringComparison.OrdinalIgnoreCase) &&
                                                                        a.Culture.Equals(currentCulture, StringComparison.OrdinalIgnoreCase)));
                
                return action.Key.Substring(prefix.Length, action.Key.Length - prefix.Length);
            }

            return translatedName.ToLowerInvariant();
        }

        private Dictionary<string, IEnumerable<TranslateAttribute>> GetTranslatedControllers(IActionDescriptorCollectionProvider provider)
        {
            var dictionary = new Dictionary<string, IEnumerable<TranslateAttribute>>();
            
            var items = provider.ActionDescriptors.Items
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
        
        private Dictionary<string, IEnumerable<TranslateAttribute>> GetTranslatedActions(IActionDescriptorCollectionProvider provider)
        {
            var dictionary = new Dictionary<string, IEnumerable<TranslateAttribute>>();
            
            var items = provider.ActionDescriptors.Items
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