using System.Collections.Generic;
using AspNetCore.Routing.Translation.Models;

namespace AspNetCore.Routing.Translation.Services
{
    public interface IRouteService
    {
        List<ICustomTranslation> RouteRules { get; }
        string GetControllerTranslatedValue(string controllerName, string culture);
        string GetControllerName(string translatedName, string currentCulture);
        string GetActionTranslatedValue(string controllerName, string actionName, string culture);
        string GetActionName(string controllerName, string translatedName, string currentCulture);
    }
}