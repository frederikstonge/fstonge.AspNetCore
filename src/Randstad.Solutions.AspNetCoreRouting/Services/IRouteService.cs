using System.Collections.Generic;
using Randstad.Solutions.AspNetCoreRouting.Models;

namespace Randstad.Solutions.AspNetCoreRouting.Services
{
    internal interface IRouteService
    {
        List<TranslationRouteRule> RouteRules { get; }
        string GetControllerTranslatedValue(string controllerName, string culture);
        string GetControllerName(string translatedName, string currentCulture);
        string GetActionTranslatedValue(string controllerName, string actionName, string culture);
        string GetActionName(string controllerName, string translatedName, string currentCulture);
    }
}