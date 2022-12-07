using System.Collections.Generic;
using fstonge.AspNetCore.Routing.Translation.Models;

namespace fstonge.AspNetCore.Routing.Translation.Services
{
    public interface IRouteService
    {
        string GetControllerTranslatedValue(string controllerName, string culture);

        string GetControllerName(string translatedName, string currentCulture);

        string GetActionTranslatedValue(string controllerName, string actionName, string culture);

        string GetActionName(string controllerName, string translatedName, string currentCulture);
    }
}