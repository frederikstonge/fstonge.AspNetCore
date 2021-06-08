using AspNetCore.Routing.Translation.Extensions;
using AspNetCore.Routing.Translation.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace SampleProject.Translations
{
    public class ProductTranslation : ICustomTranslation
    {
        private RequestLocalizationOptions _options;
        public ProductTranslation(IOptions<RequestLocalizationOptions> options)
        {
            _options = options.Value;
        }
        
        public string ControllerName => "products";
        
        public string ActionName => "detail";
        
        public RewriteRule[] RewriteRules => new[]
        {
            new RewriteRule(
                @"^([-a-zA-Z]+)\/([^\/]+)\/[-\/a-zA-Z0-9]+\/p-([=._a-zA-Z0-9]+)-.*$",
                "$1/$2/detail/$3")
        };
        
        public ICustomTranslation.GenerateUrlPath GenerateUrlPathCallback => 
            (values, _) =>
            {
                var culture = _options.SupportedCultures.Count > 1
                    ? $"{values.GetParameterValue(RouteValue.Culture)}/"
                    : string.Empty;
                
                return "/" +
                   $"{culture}" +
                   $"{values.GetParameterValue(RouteValue.Controller)}/" + 
                   "10-control-and-testing/" +
                   "14-testing-string/" +
                   $"p-{values.GetParameterValue(RouteValue.Id)}-testing-product-string";
        };
    }
}