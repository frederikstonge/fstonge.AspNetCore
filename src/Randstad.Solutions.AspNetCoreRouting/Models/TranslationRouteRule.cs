using System;
using Microsoft.AspNetCore.Routing;

namespace Randstad.Solutions.AspNetCoreRouting.Models
{
    public class TranslationRouteRule
    {
        public TranslationRouteRule(
            string controller,
            string action,
            TranslationRewriteRule[] rewriteRules,
            Func<string, string, RouteValueDictionary, string> generateUrlPath)
        {
            Controller = controller;
            Action = action;
            RewriteRules = rewriteRules;
            GenerateUrlPath = generateUrlPath;
        }
        
        public string Controller { get; }

        public string Action { get; }

        public TranslationRewriteRule[] RewriteRules { get; }

        public Func<string, string, RouteValueDictionary, string> GenerateUrlPath { get; }
    }
}