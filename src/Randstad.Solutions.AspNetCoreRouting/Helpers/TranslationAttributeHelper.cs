using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Randstad.Solutions.AspNetCoreRouting.Attributes;

namespace Randstad.Solutions.AspNetCoreRouting.Helpers
{
    public static class TranslationAttributeHelper
    {
        private const string ControllerSuffix = "Controller";
        private const string ActionSuffix = "Async";
        
        public static string GetControllerName(string controllerName, string culture)
        {
            var controllers = GetTranslatedControllers();
            var controller = controllers.Keys.FirstOrDefault(k =>
                k.Equals(controllerName + ControllerSuffix, StringComparison.OrdinalIgnoreCase));
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

        public static string GetControllerFromTranslatedValue(string translatedName, string currentCulture)
        {
            var controllers = GetTranslatedControllers();
            if (controllers.Any(c => c.Value.Any(a =>
                a.Value.Equals(translatedName, StringComparison.OrdinalIgnoreCase) &&
                a.Culture.Equals(currentCulture, StringComparison.OrdinalIgnoreCase))))
            {
                var controller = controllers.FirstOrDefault(c => c.Value.Any(a =>
                    a.Value.Equals(translatedName, StringComparison.OrdinalIgnoreCase) &&
                    a.Culture.Equals(currentCulture, StringComparison.OrdinalIgnoreCase)));

                if (controller.Key.EndsWith(ControllerSuffix))
                {
                    return controller.Key.Substring(0, controller.Key.Length - ControllerSuffix.Length).ToLower();
                }
            }

            return translatedName;
        }
        
        public static string GetActionName(string actionName, string culture)
        {
            var actions = GetTranslatedActions();
            var action = actions.Keys.FirstOrDefault(k =>
                k.Equals(actionName, StringComparison.OrdinalIgnoreCase) ||
                k.Equals(actionName + ActionSuffix, StringComparison.OrdinalIgnoreCase));
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

        public static string GetActionFromTranslatedValue(string translatedName, string currentCulture)
        {
            var actions = GetTranslatedActions();
            if (actions.Any(c => c.Value.Any(a =>
                a.Value.Equals(translatedName, StringComparison.OrdinalIgnoreCase) &&
                a.Culture.Equals(currentCulture, StringComparison.OrdinalIgnoreCase))))
            {
                var action = actions.FirstOrDefault(c => c.Value.Any(a =>
                    a.Value.Equals(translatedName, StringComparison.OrdinalIgnoreCase) &&
                    a.Culture.Equals(currentCulture, StringComparison.OrdinalIgnoreCase)));
                if (action.Key.EndsWith(ActionSuffix))
                {
                    return action.Key.Substring(0, action.Key.Length - ActionSuffix.Length).ToLower();
                }
                else
                {
                    return action.Key;
                }
            }

            return translatedName;
        }

        private static Dictionary<string, IEnumerable<TranslateAttribute>> GetTranslatedControllers()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                return assembly.GetTypes()
                    .Where(c => typeof(Controller).IsAssignableFrom(c) &&
                                c.GetCustomAttributes(typeof(TranslateAttribute), true).Any())
                    .ToDictionary(
                        k => k.Name,
                        v => v.GetCustomAttributes(typeof(TranslateAttribute), true).OfType<TranslateAttribute>());
            }

            return default;
        }
        
        private static Dictionary<string, IEnumerable<TranslateAttribute>> GetTranslatedActions()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                return assembly.GetTypes()
                    .Where(c => typeof(Controller).IsAssignableFrom(c))
                    .SelectMany(c => c.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(m => m.GetCustomAttributes(typeof(TranslateAttribute), true).Any())
                        .ToDictionary(
                        k => k.Name, 
                        v => v.GetCustomAttributes(typeof(TranslateAttribute), true).OfType<TranslateAttribute>());
            }

            return default;
        }
    }
}