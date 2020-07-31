using AspNetCore.Routing.Translation.Extensions;
using AspNetCore.Routing.Translation.Models;

namespace MvcApp.Translations
{
    public class ProductTranslation : ICustomTranslation
    {
        public ProductTranslation()
        {
            RewriteRules = new[]
            {
                new RewriteRule(
                    @"^([a-zA-Z]{2})\/([^\/]+)\/[-\/a-zA-Z0-9]+\/p-([=._a-zA-Z0-9]+)-.*$",
                    "$1/$2/detail/$3"),
            };
            
            GenerateUrlPathCallback = 
                (culture, controllerValue, actionValue, values, fragment) =>
                {
                    var id = values.GetParameterValue("id");

                    return $"/{culture}/" +
                           $"{controllerValue}/" + 
                           $"10-control-and-testing/" +
                           $"14-testing-string/" +
                           $"p-{id}-testing-product-string";
                };
        }
        public string ControllerName => "products";
        public string ActionName => "detail";
        public RewriteRule[] RewriteRules { get; }
        public ICustomTranslation.GenerateUrlPath GenerateUrlPathCallback { get; }
    }
}