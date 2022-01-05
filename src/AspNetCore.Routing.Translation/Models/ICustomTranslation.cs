using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.Routing.Translation.Models
{
    public interface ICustomTranslation
    {
        string ControllerName { get; }
        
        string ActionName { get; }
        
        RewriteRule[] RewriteRules { get; }

        string GenerateUrlPath(RouteValueDictionary values, FragmentString fragment);
    }
}