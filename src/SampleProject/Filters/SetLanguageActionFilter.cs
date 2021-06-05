using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SampleProject.Filters
{
    public class SetLanguageActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var rqf = context.HttpContext.Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var culture = rqf.RequestCulture.Culture.TwoLetterISOLanguageName;

            if (context.HttpContext.User.Identity is ClaimsIdentity identity)
            {
                var currentSessionCulture = identity.FindFirst("language")?.Value;

                if (currentSessionCulture != null &&
                    !culture.Equals(currentSessionCulture, StringComparison.OrdinalIgnoreCase))
                {
                    // Set new culture in session
                }
            }

            await next();
        }
    }
}