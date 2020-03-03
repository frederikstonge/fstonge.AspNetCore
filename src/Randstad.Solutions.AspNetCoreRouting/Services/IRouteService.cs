using System.Collections.Generic;
using Randstad.Solutions.AspNetCoreRouting.Models;

namespace Randstad.Solutions.AspNetCoreRouting.Services
{
    internal interface IRouteService
    {
        List<TranslationRouteRule> RouteRules { get; set; }
        string GetControllerTranslatedValue(string controllerName, string culture);
        string GetControllerNameFromTranslatedValue(string translatedName, string currentCulture);
        string GetActionTranslatedValue(string actionName, string culture);
        string GetActionNameFromTranslatedValue(string translatedName, string currentCulture);
    }
}