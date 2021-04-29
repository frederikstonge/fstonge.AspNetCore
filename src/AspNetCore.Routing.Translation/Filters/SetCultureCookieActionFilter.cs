using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCore.Routing.Translation.Filters
{
    internal class SetCultureCookieActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
             var rqf = context.HttpContext.Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var culture = rqf.RequestCulture.Culture.TwoLetterISOLanguageName;
            context.HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    IsEssential = true
                });
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}