using System.Collections.Generic;
using SoneparCanada.OpenCatalog.AspNetCoreRouting.Models;

namespace SoneparCanada.OpenCatalog.AspNetCoreRouting.Services
{
    internal interface IRouteService
    {
        List<CustomTranslation> RouteRules { get; }
        string GetControllerTranslatedValue(string controllerName, string culture);
        string GetControllerName(string translatedName, string currentCulture);
        string GetActionTranslatedValue(string controllerName, string actionName, string culture);
        string GetActionName(string controllerName, string translatedName, string currentCulture);
    }
}