using System;
using Microsoft.AspNetCore.Routing;

namespace Randstad.Solutions.AspNetCoreRouting.Models
{
    public abstract class TranslationRouteRule
    {
        public string Controller { get; }

        public string Action { get; }

        public TranslationRewriteRule[] RewriteRules { get; set; }
        
        public abstract string GenerateUrlPath(string controller, string action, RouteValueDictionary data);
    }
}