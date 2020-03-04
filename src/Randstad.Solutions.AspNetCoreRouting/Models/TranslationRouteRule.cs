using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Randstad.Solutions.AspNetCoreRouting.Models
{
    public class TranslationRouteRule
    {
        public TranslationRouteRule(
            string controller,
            string action,
            TranslationRewriteRule[] rewriteRules,
            GenerateUrlPath generateUrlPathCallback)
        {
            ControllerName = controller;
            ActionName = action;
            RewriteRules = rewriteRules;
            GenerateUrlPathCallback = generateUrlPathCallback;
        }
        
        public string ControllerName { get; }

        public string ActionName { get; }

        public TranslationRewriteRule[] RewriteRules { get; }

        public GenerateUrlPath GenerateUrlPathCallback { get; }

        public delegate string GenerateUrlPath(
            string controllerValue,
            string actionValue, 
            RouteValueDictionary values,
            RouteValueDictionary ambiantValues,
            FragmentString fragment);
    }
}