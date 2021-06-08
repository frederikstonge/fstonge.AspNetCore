using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Routing.Translation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace AspNetCore.Routing.Translation.Filters
{
    internal class SetCultureCookieActionFilter : IAsyncActionFilter
    {
        private readonly RoutingTranslationOptions _options;
        
        public SetCultureCookieActionFilter(IOptions<RoutingTranslationOptions> options)
        {
            _options = options.Value;
        }
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var rqf = context.HttpContext.Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var culture = rqf.RequestCulture.Culture;
            var currentCulture = _options.SupportedCultures
                                     .FirstOrDefault(c => new CultureInfo(c).Equals(culture))
                                 ?? _options.DefaultCulture;
            
            context.HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(currentCulture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    IsEssential = true
                });

            await next();
        }
    }
}