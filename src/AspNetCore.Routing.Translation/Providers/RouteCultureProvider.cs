using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

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
                if (!string.IsNullOrWhiteSpace(culture))
                {
                    try
                    {
                        var currentCulture = new CultureInfo(culture);
                        if (Options.SupportedCultures.Any(l => l.Equals(currentCulture)))
                        {
                            // Set Culture and UICulture from route culture parameter
                            return await Task.FromResult(new ProviderCultureResult(culture, culture));
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            return await NullProviderCultureResult;
        }
    }
}
