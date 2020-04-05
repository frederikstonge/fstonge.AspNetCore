using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Randstad.Solutions.AspNetCoreRouting.Models
{
    public class CustomTranslation
    {
        public CustomTranslation(
            string controller,
            string action,
            RewriteRule[] rewriteRules,
            GenerateUrlPath generateUrlPathCallback)
        {
            ControllerName = controller;
            ActionName = action;
            RewriteRules = rewriteRules;
            GenerateUrlPathCallback = generateUrlPathCallback;
        }
        
        public string ControllerName { get; }

        public string ActionName { get; }

        public RewriteRule[] RewriteRules { get; }

        public GenerateUrlPath GenerateUrlPathCallback { get; }

        public delegate string GenerateUrlPath(
            string culure,
            string controllerValue,
            string actionValue, 
            RouteValueDictionary values,
            FragmentString fragment);
    }
}