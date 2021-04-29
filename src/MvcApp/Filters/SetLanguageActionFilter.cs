using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MvcApp.Filters
{
    public class SetLanguageActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var rqf = context.HttpContext.Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var culture = rqf.RequestCulture.Culture.TwoLetterISOLanguageName;

            var currentSessionCulture = "en"; //Get session to get language

            if (!culture.Equals(currentSessionCulture, StringComparison.OrdinalIgnoreCase))
            {
                // Set new culture in session
            }
            
            await next();
        }
    }
}