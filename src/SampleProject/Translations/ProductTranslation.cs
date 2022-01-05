using AspNetCore.Routing.Translation.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace SampleProject.Translations
{
    public class ProductTranslation : ICustomTranslation
    {
        private readonly RequestLocalizationOptions _options;
        public ProductTranslation(IOptions<RequestLocalizationOptions> options)
        {
            _options = options.Value;
        }
        
        public string ControllerName => "products";
        
        public string ActionName => "detail";
        
        public RewriteRule[] RewriteRules => new[]
        {
            _options.SupportedCultures.Count > 1
                ? new RewriteRule(
                    @"^([-a-zA-Z]+)\/([^\/]+)\/[-\/a-zA-Z0-9]+\/p-([=._a-zA-Z0-9]+)-.*$",
                    "$1/$2/detail/$3")
                : new RewriteRule(
                    @"^([^\/]+)\/[-\/a-zA-Z0-9]+\/p-([=._a-zA-Z0-9]+)-.*$",
                    "$1/detail/$2")
        };

        public string GenerateUrlPath(RouteValueDictionary values, FragmentString fragment)
        {
            var culture = _options.SupportedCultures.Count > 1
                ? $"{values[RouteValue.Culture]}/"
                : string.Empty;
                
            return "/" +
                   $"{culture}" +
                   $"{values[RouteValue.Controller]}/" + 
                   "10-control-and-testing/" +
                   "14-testing-string/" +
                   $"p-{values[RouteValue.Id]}-testing-product-string";
        }
    }
}