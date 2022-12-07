using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace fstonge.AspNetCore.Routing.Translation.Providers
{
    public class RouteCultureProvider : RequestCultureProvider
    {
        private static readonly Regex CultureRegex = new (@"^\/([-a-zA-Z]+).*$");

        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (httpContext.Request.Path.Value != null)
            {
                var match = CultureRegex.Match(httpContext.Request.Path.Value);
                if (match.Success && match.Groups.Count == 2)
                {
                    try
                    {
                        var culture = match.Groups[1].Value;
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
