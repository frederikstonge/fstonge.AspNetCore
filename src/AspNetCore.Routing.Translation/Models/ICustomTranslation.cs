using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.Routing.Translation.Models
{
    public interface ICustomTranslation
    {
        string ControllerName { get; }
        string ActionName { get; }
        RewriteRule[] RewriteRules { get; }
        GenerateUrlPath GenerateUrlPathCallback { get; }
        
        public delegate string GenerateUrlPath(
            string culure,
            string controllerValue,
            string actionValue, 
            RouteValueDictionary values,
            FragmentString fragment);
    }
}