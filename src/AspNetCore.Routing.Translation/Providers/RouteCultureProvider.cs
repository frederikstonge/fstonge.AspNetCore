using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Routing.Translation.Providers
{
    public class RouteCultureProvider : RequestCultureProvider
    {
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (httpContext.Request.Path.Value != null)
            {
                var paths = httpContext.Request.Path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var culture = paths.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(culture) &&
                    Options.SupportedCultures.Any(l => l.TwoLetterISOLanguageName.Equals(culture, StringComparison.OrdinalIgnoreCase)))
                {
                    // Set Culture and UICulture from route culture parameter
                    return await Task.FromResult(new ProviderCultureResult(culture, culture));
                }
            }

            return await NullProviderCultureResult;
        }
    }
}
